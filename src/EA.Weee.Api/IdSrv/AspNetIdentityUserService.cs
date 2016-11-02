/*
 * Copyright 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace EA.Weee.Api.IdSrv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer3.Core.Extensions;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services.Default;
    using Microsoft.AspNet.Identity;
    using Constants = IdentityServer3.Core.Constants;

    public class AspNetIdentityUserService<TUser, TKey> : UserServiceBase
        where TUser : class, IUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly Func<string, TKey> ConvertSubjectToKey;

        protected readonly UserManager<TUser, TKey> UserManager;

        public AspNetIdentityUserService(UserManager<TUser, TKey> userManager, Func<string, TKey> parseSubject = null)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }

            this.UserManager = userManager;

            if (parseSubject != null)
            {
                ConvertSubjectToKey = parseSubject;
            }
            else
            {
                var keyType = typeof(TKey);
                if (keyType == typeof(string))
                {
                    ConvertSubjectToKey = subject => (TKey)ParseString(subject);
                }
                else if (keyType == typeof(int))
                {
                    ConvertSubjectToKey = subject => (TKey)ParseInt(subject);
                }
                else if (keyType == typeof(uint))
                {
                    ConvertSubjectToKey = subject => (TKey)ParseUInt32(subject);
                }
                else if (keyType == typeof(long))
                {
                    ConvertSubjectToKey = subject => (TKey)ParseLong(subject);
                }
                else if (keyType == typeof(Guid))
                {
                    ConvertSubjectToKey = subject => (TKey)ParseGuid(subject);
                }
                else
                {
                    throw new InvalidOperationException("Key type not supported");
                }
            }

            EnableSecurityStamp = true;
        }

        public string DisplayNameClaimType { get; set; }

        public bool EnableSecurityStamp { get; set; }

        private static object ParseString(string sub)
        {
            return sub;
        }

        private static object ParseInt(string sub)
        {
            int key;
            if (!int.TryParse(sub, out key))
            {
                return 0;
            }
            return key;
        }

        private static object ParseUInt32(string sub)
        {
            uint key;
            if (!uint.TryParse(sub, out key))
            {
                return 0;
            }
            return key;
        }

        private static object ParseLong(string sub)
        {
            long key;
            if (!long.TryParse(sub, out key))
            {
                return 0;
            }
            return key;
        }

        private static object ParseGuid(string sub)
        {
            Guid key;
            if (!Guid.TryParse(sub, out key))
            {
                return Guid.Empty;
            }
            return key;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext ctx)
        {
            var subject = ctx.Subject;
            var requestedClaimTypes = ctx.RequestedClaimTypes;

            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var key = ConvertSubjectToKey(subject.GetSubjectId());
            var acct = await UserManager.FindByIdAsync(key);
            if (acct == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            var claims = await GetClaimsFromAccount(acct);
            if (requestedClaimTypes != null && requestedClaimTypes.Any())
            {
                claims = claims.Where(x => requestedClaimTypes.Contains(x.Type));
            }

            ctx.IssuedClaims = claims;
        }

        protected virtual async Task<IEnumerable<Claim>> GetClaimsFromAccount(TUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.Subject, user.Id.ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, user.UserName)
            };

            if (UserManager.SupportsUserEmail)
            {
                var email = await UserManager.GetEmailAsync(user.Id);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    claims.Add(new Claim(Constants.ClaimTypes.Email, email));
                    var verified = await UserManager.IsEmailConfirmedAsync(user.Id);
                    claims.Add(new Claim(Constants.ClaimTypes.EmailVerified, verified ? "true" : "false"));
                }
            }

            if (UserManager.SupportsUserPhoneNumber)
            {
                var phone = await UserManager.GetPhoneNumberAsync(user.Id);
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    claims.Add(new Claim(Constants.ClaimTypes.PhoneNumber, phone));
                    var verified = await UserManager.IsPhoneNumberConfirmedAsync(user.Id);
                    claims.Add(new Claim(Constants.ClaimTypes.PhoneNumberVerified, verified ? "true" : "false"));
                }
            }

            if (UserManager.SupportsUserClaim)
            {
                claims.AddRange(await UserManager.GetClaimsAsync(user.Id));
            }

            if (UserManager.SupportsUserRole)
            {
                var roleClaims =
                    from role in await UserManager.GetRolesAsync(user.Id)
                    select new Claim(Constants.ClaimTypes.Role, role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }

        protected virtual async Task<string> GetDisplayNameForAccountAsync(TKey userID)
        {
            var user = await UserManager.FindByIdAsync(userID);
            var claims = await GetClaimsFromAccount(user);

            Claim nameClaim = null;
            if (DisplayNameClaimType != null)
            {
                nameClaim = claims.FirstOrDefault(x => x.Type == DisplayNameClaimType);
            }
            if (nameClaim == null)
            {
                nameClaim = claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);
            }
            if (nameClaim == null)
            {
                nameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            }
            if (nameClaim != null)
            {
                return nameClaim.Value;
            }

            return user.UserName;
        }

        protected virtual async Task<TUser> FindUserAsync(string username)
        {
            return await UserManager.FindByNameAsync(username);
        }

        protected virtual Task<AuthenticateResult> PostAuthenticateLocalAsync(TUser user, SignInMessage message)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext ctx)
        {
            var username = ctx.UserName;
            var password = ctx.Password;
            var message = ctx.SignInMessage;

            ctx.AuthenticateResult = null;

            if (UserManager.SupportsUserPassword)
            {
                var user = await FindUserAsync(username);
                if (user != null)
                {
                    if (UserManager.SupportsUserLockout &&
                        await UserManager.IsLockedOutAsync(user.Id))
                    {
                        return;
                    }

                    if (await UserManager.CheckPasswordAsync(user, password))
                    {
                        if (UserManager.SupportsUserLockout)
                        {
                            await UserManager.ResetAccessFailedCountAsync(user.Id);
                        }

                        var result = await PostAuthenticateLocalAsync(user, message);
                        if (result == null)
                        {
                            var claims = await GetClaimsForAuthenticateResult(user);
                            result = new AuthenticateResult(user.Id.ToString(),
                                await GetDisplayNameForAccountAsync(user.Id), claims);
                        }

                        ctx.AuthenticateResult = result;
                    }
                    else if (UserManager.SupportsUserLockout)
                    {
                        await UserManager.AccessFailedAsync(user.Id);
                    }
                }
            }
        }

        protected virtual async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResult(TUser user)
        {
            var claims = new List<Claim>();
            if (EnableSecurityStamp && UserManager.SupportsUserSecurityStamp)
            {
                var stamp = await UserManager.GetSecurityStampAsync(user.Id);
                if (!string.IsNullOrWhiteSpace(stamp))
                {
                    claims.Add(new Claim("security_stamp", stamp));
                }
            }
            return claims;
        }

        public override async Task AuthenticateExternalAsync(ExternalAuthenticationContext ctx)
        {
            var externalUser = ctx.ExternalIdentity;
            var message = ctx.SignInMessage;

            if (externalUser == null)
            {
                throw new ArgumentNullException("externalUser");
            }

            var user = await UserManager.FindAsync(new UserLoginInfo(externalUser.Provider, externalUser.ProviderId));
            if (user == null)
            {
                ctx.AuthenticateResult =
                    await
                        ProcessNewExternalAccountAsync(externalUser.Provider, externalUser.ProviderId,
                            externalUser.Claims);
            }
            else
            {
                ctx.AuthenticateResult =
                    await
                        ProcessExistingExternalAccountAsync(user.Id, externalUser.Provider, externalUser.ProviderId,
                            externalUser.Claims);
            }
        }

        protected virtual async Task<AuthenticateResult> ProcessNewExternalAccountAsync(string provider,
            string providerId, IEnumerable<Claim> claims)
        {
            var user = await TryGetExistingUserFromExternalProviderClaimsAsync(provider, claims);
            if (user == null)
            {
                user = await InstantiateNewUserFromExternalProviderAsync(provider, providerId, claims);
                if (user == null)
                {
                    throw new InvalidOperationException("CreateNewAccountFromExternalProvider returned null");
                }

                var createResult = await UserManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new AuthenticateResult(createResult.Errors.First());
                }
            }

            var externalLogin = new UserLoginInfo(provider, providerId);
            var addExternalResult = await UserManager.AddLoginAsync(user.Id, externalLogin);
            if (!addExternalResult.Succeeded)
            {
                return new AuthenticateResult(addExternalResult.Errors.First());
            }

            var result = await AccountCreatedFromExternalProviderAsync(user.Id, provider, providerId, claims);
            if (result != null)
            {
                return result;
            }

            return await SignInFromExternalProviderAsync(user.Id, provider);
        }

        protected virtual Task<TUser> InstantiateNewUserFromExternalProviderAsync(string provider, string providerId,
            IEnumerable<Claim> claims)
        {
            var user = new TUser { UserName = Guid.NewGuid().ToString("N") };
            return Task.FromResult(user);
        }

        protected virtual Task<TUser> TryGetExistingUserFromExternalProviderClaimsAsync(string provider,
            IEnumerable<Claim> claims)
        {
            return Task.FromResult<TUser>(null);
        }

        protected virtual async Task<AuthenticateResult> AccountCreatedFromExternalProviderAsync(TKey userID,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            claims = await SetAccountEmailAsync(userID, claims);
            claims = await SetAccountPhoneAsync(userID, claims);

            return await UpdateAccountFromExternalClaimsAsync(userID, provider, providerId, claims);
        }

        protected virtual async Task<AuthenticateResult> SignInFromExternalProviderAsync(TKey userID, string provider)
        {
            var user = await UserManager.FindByIdAsync(userID);
            var claims = await GetClaimsForAuthenticateResult(user);

            return new AuthenticateResult(
                userID.ToString(),
                await GetDisplayNameForAccountAsync(userID),
                claims,
                authenticationMethod: Constants.AuthenticationMethods.External,
                identityProvider: provider);
        }

        protected virtual async Task<AuthenticateResult> UpdateAccountFromExternalClaimsAsync(TKey userID,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            var existingClaims = await UserManager.GetClaimsAsync(userID);
            var intersection = existingClaims.Intersect(claims, new ClaimComparer());
            var newClaims = claims.Except(intersection, new ClaimComparer());

            foreach (var claim in newClaims)
            {
                var result = await UserManager.AddClaimAsync(userID, claim);
                if (!result.Succeeded)
                {
                    return new AuthenticateResult(result.Errors.First());
                }
            }

            return null;
        }

        protected virtual async Task<AuthenticateResult> ProcessExistingExternalAccountAsync(TKey userID,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            return await SignInFromExternalProviderAsync(userID, provider);
        }

        protected virtual async Task<IEnumerable<Claim>> SetAccountEmailAsync(TKey userID, IEnumerable<Claim> claims)
        {
            var email = claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Email);
            if (email != null)
            {
                var userEmail = await UserManager.GetEmailAsync(userID);
                if (userEmail == null)
                {
                    // if this fails, then presumably the email is already associated with another account
                    // so ignore the error and let the claim pass thru
                    var result = await UserManager.SetEmailAsync(userID, email.Value);
                    if (result.Succeeded)
                    {
                        var email_verified = claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.EmailVerified);
                        if (email_verified != null && email_verified.Value == "true")
                        {
                            var token = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
                            await UserManager.ConfirmEmailAsync(userID, token);
                        }

                        var emailClaims = new[] { Constants.ClaimTypes.Email, Constants.ClaimTypes.EmailVerified };
                        return claims.Where(x => !emailClaims.Contains(x.Type));
                    }
                }
            }

            return claims;
        }

        protected virtual async Task<IEnumerable<Claim>> SetAccountPhoneAsync(TKey userID, IEnumerable<Claim> claims)
        {
            var phone = claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.PhoneNumber);
            if (phone != null)
            {
                var userPhone = await UserManager.GetPhoneNumberAsync(userID);
                if (userPhone == null)
                {
                    // if this fails, then presumably the phone is already associated with another account
                    // so ignore the error and let the claim pass thru
                    var result = await UserManager.SetPhoneNumberAsync(userID, phone.Value);
                    if (result.Succeeded)
                    {
                        var phone_verified =
                            claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.PhoneNumberVerified);
                        if (phone_verified != null && phone_verified.Value == "true")
                        {
                            var token = await UserManager.GenerateChangePhoneNumberTokenAsync(userID, phone.Value);
                            await UserManager.ChangePhoneNumberAsync(userID, phone.Value, token);
                        }

                        var phoneClaims = new[]
                        {
                            Constants.ClaimTypes.PhoneNumber, Constants.ClaimTypes.PhoneNumberVerified
                        };
                        return claims.Where(x => !phoneClaims.Contains(x.Type));
                    }
                }
            }

            return claims;
        }

        public override async Task IsActiveAsync(IsActiveContext ctx)
        {
            var subject = ctx.Subject;

            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var id = subject.GetSubjectId();
            var key = ConvertSubjectToKey(id);
            var acct = await UserManager.FindByIdAsync(key);

            ctx.IsActive = false;

            if (acct != null)
            {
                if (EnableSecurityStamp && UserManager.SupportsUserSecurityStamp)
                {
                    var security_stamp =
                        subject.Claims.Where(x => x.Type == "security_stamp").Select(x => x.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await UserManager.GetSecurityStampAsync(key);
                        if (db_security_stamp != security_stamp)
                        {
                            return;
                        }
                    }
                }

                ctx.IsActive = true;
            }
        }
    }
}