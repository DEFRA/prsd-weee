src\EA.Weee.Database\scripts\AliaSQL.exe Rebuild PRORESDBDEV WeeeTest src\EA.Weee.Database\scripts weeeadmin weeeadmin

powershell -command "&tools\Scripts\add-database-user.ps1 -ConnectionString 'Data Source=PRORESDBDEV;Initial Catalog=WeeeTest;User Id=weeeadmin;Password=weeeadmin' -Username 'weeeapplicationuser' -Login 'weeeapplicationuser' -Role 'weee_application'

powershell -command "&tools\Scripts\add-internal-user.ps1 -ConnectionString 'Data Source=PRORESDBDEV;Initial Catalog=WeeeTest;User Id=weeeadmin;Password=weeeadmin' -FirstName 'SUPPORT' -Surname 'INTERNAL USER' -Email 'chriss+internal@sfwltd.co.uk' -HashedPassword 'AAxQREIFH7nE1sqVDM/oX8NUQjeG+ypTPde58F/OfANWVtRDZyTcLZDBN2Dx8v+i8g==' -SecurityStamp '3ae59998-f529-4d0c-872a-6b808eeb6946'

powershell -command "&tools\Scripts\add-external-user.ps1 -ConnectionString 'Data Source=PRORESDBDEV;Initial Catalog=WeeeTest;User Id=weeeadmin;Password=weeeadmin' -FirstName 'ApiWarmUpUser' -Surname 'ApiWarmUpUser' -Email 'ApiWarmUpUser@weee.dev' -HashedPassword 'AJX9xPQLCuRezIbF1FVsZKXo8cTrN7NVkEPngA8TorSWW5ZxwNfJ3U2CYuHiM5Jd6A==' -SecurityStamp '58e8bd8e-769e-4545-84de-5780f7e9c937'

powershell -command "&tools\Scripts\update-competent-authority-emails.ps1 -ConnectionString 'Data Source=PRORESDBDEV;Initial Catalog=WeeeTest;User Id=weeeadmin;Password=weeeadmin' -EAMail 'weee.test.ea@gmail.com' -SEPAMail 'weee.test.sepa@gmail.com' -NIEAMail 'weee.test.niea@gmail.com' -NRWMail 'weee.test.nrw@gmail.com'

powershell -command "&tools\Scripts\set-fixed-date-for-submissions.ps1 -ConnectionString 'Data Source=PRORESDBDEV;Initial Catalog=WeeeTest;User Id=weeeadmin;Password=weeeadmin' -FixDateForSubmissions $True -DateForSubmissions '2018-04-01'"
