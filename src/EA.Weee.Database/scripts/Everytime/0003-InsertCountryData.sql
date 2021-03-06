﻿
GO
PRINT N'Altering [Lookup].[Country]...';

DECLARE @tempCountryTable TABLE (
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    [Name]  NVARCHAR (2048)  NOT NULL
)

INSERT INTO @tempCountryTable([Id],[Name])VALUES('CF57BDAA-BCF0-48C5-AC6F-662D24DF68BF','Afghanistan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A5D22C34-F99F-4811-9C58-A1AC1989B47C','Åland Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('81E1738E-1CFE-4625-9DF8-90FFD5DFD943','Albania')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CF05A8B0-EA89-4B6D-9ECF-599E94371D98','Algeria')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5223FB91-CC90-44E9-8383-94FF383BAA00','American Samoa')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2F36FDCC-6EED-48FA-B9B2-1B5E3D4BDFEE','Andorra')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('48F80F67-02F7-4B1C-A9B6-D268FB855CAC','Angola')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DB1A8BE4-80F1-4940-B006-11BA74C4E649','Anguilla')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('44417DDA-C5A1-4884-9A01-8E0CACC865FB','Antarctica')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('75AE079B-3299-4DE1-A7BC-D4590ADB0B37','Antigua and Barbuda')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('015C233B-F7C6-4E87-8C9D-509CBE19B507','Argentina')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0BFEF5C2-3E28-4E08-BA89-B387C39D6718','Armenia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9950FC05-804A-4287-9E64-8C6DD9E9032A','Aruba')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DDC6551C-D9B2-465C-86DD-670B7D2142C2','Australia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('1A3414CF-D96E-4926-9B7A-2F8EA59BD98F','Austria')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B5EBE1D1-8349-43CD-9E87-0081EA0A8463','Azerbaijan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0BB8B6A4-1B19-4C76-B7B6-D6659A5A134B','Bahamas')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('48FACFEF-22F5-4918-9098-FC545EFF7EF8','Bahrain')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('699086D0-7031-4EC9-85E2-FFA836BF7B05','Bangladesh')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('1D6B469F-6A54-4440-8796-5468D6987912','Barbados')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('703C690C-825B-4C44-9A23-3688FF4697B9','Belarus')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E10118B3-0FEB-40F5-827F-61AE3520611E','Belgium')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5954474E-66C1-4AE2-859E-F37509DC3DA6','Belize')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('31A0C2FA-D9E0-43AC-8674-BA9E5EBE7635','Benin')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('306C4714-C207-4077-A742-057A20FF0BF3','Bermuda')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B89A375E-0CDA-4466-A65E-94E7A65CF86B','Bhutan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('49DECA1E-2ED1-4F31-B0C7-D1B3D3409868','Bolivia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('64C8ABAD-3490-4F88-8A48-69B22FF484F3','Bonaire, Sint Eustatius and Saba')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AB8A3ABF-C0F7-4735-B145-812D0185FC10','Bosnia and Herzegovina')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0DD1CDD0-4225-44C0-AD7E-A606089D492E','Botswana')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3D442C19-CFCC-4785-ACBA-01B4A9C7496E','Bouvet Island')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B73C4F7F-B946-4828-84A9-92FC9F81C3B9','Brazil')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CB866D5C-7817-4B27-9D8A-8A92FAB61488','British Indian Ocean Territory')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0321BD73-4E02-4F4B-9847-412EA3EF8A34','Brunei Darussalam')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B45070D6-A073-413B-9883-5E181895D5AC','Bulgaria')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AF5F5919-5DA1-441E-938A-3A6D307F4C13','Burkina Faso')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('62DCE536-941C-4F14-ABD8-818B41CEB864','Burundi')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2889C722-75A9-4CAC-AB66-2B1534DC7374','Cabo Verde')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('24C8B1DF-AEF1-4720-ACC6-C1A80202D273','Cambodia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0B3C6F53-A968-42BB-8784-BEDB03CDFADF','Cameroon')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0242EBCF-7F85-4DBB-A420-3CB9BCA798C9','Canada')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CCB802EB-375F-4269-8440-DBC2B5A0C1FA','Cayman Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0B89732E-4B33-421F-9352-F58D825C8A69','Central African Republic')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('299325A9-5BC9-4041-A967-68829542D4FF','Chad')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8158F05A-D00B-47AF-8AD1-1FF457CDB0C9','Chile')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('21168CC4-0933-412E-876C-654F2381F3FD','China')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3E0184C2-35BA-4941-ADE1-A8800B44FE14','Christmas Island')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9DB39510-4EE9-4FE6-9385-D42E2EE4A736','Cocos (Keeling) Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C4B471CB-F71B-4906-85F1-D48D25825028','Colombia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A708894D-CFBE-471C-8E59-B74CDCF39C26','Comoros')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('73848791-D517-44D7-BA7D-4408522081DC','Congo')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('60A9FAEC-F0AD-4E8D-8FED-A57370D46AEE','Congo, Democratic Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9E7AA137-71AE-4395-83A9-C3D0B08DF7A8','Cook Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('85D806EE-615A-44B8-A904-7F1F164036D4','Costa Rica')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5442BC62-88CC-42EA-BD34-FF939914912B','Côte d''Ivoire')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3DE3A637-BF2D-4691-BF38-0BF917FDB5F7','Croatia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7BE7D3F4-C984-44EF-B7CF-72614CC64707','Cuba')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('01244B26-E11B-4824-8F6F-CECEF8F11F86','Curaçao')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('22902E7E-42CA-4643-BDA2-86387CCB65D3','Cyprus')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AC5B6ADB-3DAB-4BDD-AB4D-3F92CBAAED02','Czech Republic')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('799B6E77-F8A2-47E0-85EB-2B6A21C3B678','Denmark')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C454CCB7-E1C7-48F4-83EC-5B4AAC472CE8','Djibouti')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C464595D-D6BC-4AC2-8565-6C9559280D25','Dominica')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E847D179-C848-470B-90C2-CD5DADD8F406','Dominican Republic')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B37B38DE-04AD-400E-BE6B-E5B3F4BB2355','Ecuador')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('04191657-43AD-4B9F-84A8-383F55AB5D88','Egypt')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E2002A68-37A9-4715-8E09-87D2EA9F20F4','El Salvador')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B30FAB3B-7E0E-4372-8EFF-F9EB01DF44CD','Equatorial Guinea')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('48140B61-BF30-4B09-A409-FE0ACA197D20','Eritrea')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CADE06A7-8C9D-4720-A92A-853DFECDB8C3','Estonia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5C39091E-DBE6-4007-B467-63697BD5505A','Ethiopia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('40FEC43D-ABBA-456F-AA8C-835D7A558364','Falkland Islands, the Malvinas')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('BF333898-E923-488D-A0DF-7F1D2CF9808B','Faroe Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A2C0A4D9-D5A5-48A8-BC57-FCB06D77979D','Fiji')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('128B59F9-43F2-40C3-AB0B-AFC38BBD0250','Finland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3EF4F2FC-A090-42C4-90E8-E2916090D2C4','France')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8EB15995-17CC-4622-8C90-F7D76C872329','French Guiana')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0272D805-18A4-45D4-934C-F589DF5CAD53','French Polynesia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('255DA01F-E0B7-4F12-91D8-0E11866F72C2','French Southern Territories')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('094B1411-6BD9-4CFE-9BF1-65B75684CEEF','Gabon')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A5998090-2831-44AC-9C9D-E113E41B8620','Gambia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('1B7134F7-1070-4FF9-ADE1-2F0A156A0AD7','Georgia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E6DD6923-C7A4-44A8-BBE2-D27201341DDB','Germany')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3DD6ACAA-D6B5-4FDE-B1AD-2BC8CDABF214','Ghana')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C915D5F8-FCC5-4A50-8ECF-BFB332F30423','Gibraltar')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5CDE6128-0106-4258-ABDC-2215E47D738F','Greece')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7D5101E1-886C-4415-8BB7-04EA786AAA29','Greenland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('77042F31-4398-47B6-8FD6-964FFE013108','Grenada')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('87085A03-A7B5-42F8-9EF5-0396E6A61C18','Guadeloupe')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2FA30452-8CE7-409D-9965-17485E606B55','Guam')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('98B99DE4-B5C5-4645-ABC0-23E8FEEDEB12','Guatemala')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4C178840-8CC6-4FFF-BCF9-873C79FA0A4C','Guernsey')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A0F80530-1985-4B78-B1A3-6FFB658706DA','Guinea')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8341C530-9665-4681-8796-25CC43D33B5E','Guinea-Bissau')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0BA5C4BF-6D02-4F37-9E8F-925355B284E6','Guyana')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('ECC19611-A814-48F0-A839-10A81D01AE34','Haiti')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4BC03F38-0650-41FF-B18A-CDB73B4721CB','Heard Island and McDonald Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('53E5F60F-4881-4D77-8399-96E7F5FCF543','Holy See')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F3930903-4E63-4FFA-B896-3E3FFA9C75EC','Honduras')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5E5A825E-F760-4274-A6EE-9D4277D4C306','Hong Kong')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3BDEA7F1-043A-41D9-9DE4-257A83AA6199','Hungary')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4D23A300-AE2C-4CC4-9CAB-750DAC493761','Iceland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('70FFB285-E370-4537-A39C-B5ADA48D3121','India')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7D0C054B-D16F-4F41-950F-32D28913DEB8','Indonesia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('771C2C73-3D21-4375-9C50-FD6CB2AD3111','Iran, Islamic Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4E17C6E3-FB25-49AE-AD6C-AA20B38404B2','Iraq')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('595A6A95-4991-4926-9E34-655FE9C17B1E','Ireland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CB46DF14-8BD9-44B7-89F4-2A62A74315AE','Isle of Man')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CB0E7142-FE1F-4FB8-83F4-4FC725200520','Israel')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AC82340A-F45F-46FF-8C10-478C6D6D16F3','Italy')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4765594D-445E-4C1E-BA8B-201189C6C850','Jamaica')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('798C89E8-BF1F-40DB-B18F-5324852D802B','Japan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3F944F2D-C2AD-4F08-B061-368456D1F839','Jersey')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('FD4161E8-4F7C-432C-8CF3-80449B8536EB','Jordan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('93C2A0F2-F398-47C0-B351-0D6318783A14','Kazakhstan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AF7AECE3-D4B6-4CB9-B31C-371950CDB5C8','Kenya')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('BC2AE316-42D1-41AA-A4AF-0803BD69A795','Kiribati')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0FE271DB-6740-41E8-95EE-665FA52DA2CE','Korea, the Democratic People''s Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('537A61DE-09BB-446B-85CB-2DD7B8C2FE28','Korea, the Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('84035B4E-E2A1-4365-B241-5741DD1AD330','Kuwait')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4645F31E-F364-4CD2-8BB2-9303DC94E30E','Kyrgyzstan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4423EA2D-9DCB-4A21-B068-6D9C7E5E6502','Lao People''s Democratic Republic')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CD5A7D7C-0AF1-400E-9C5D-F437CDEBDB11','Latvia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4A1575DE-A992-48BB-BD00-450482DADBA1','Lebanon')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A1A45927-2A08-4AA5-AB6B-AC7B25D0E963','Lesotho')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C078FA8A-CBA7-4546-B533-DF76C9F17BC8','Liberia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('294B9B2D-6F71-4AD4-BD6E-6F8708A4209B','Libya')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('34DCED88-6E38-47F2-9686-3F2F87203E7F','Liechtenstein')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('54BF9FEE-7E70-4ED0-AE80-2ACA5EC1E83B','Lithuania')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('33947377-48B6-4F9E-BF99-9C9E72C9AD76','Luxembourg')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('01C87B74-25BE-4631-84B4-A9CE26EF0050','Macau')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2F0443E8-B680-40CF-B1AA-F78D65C34110','Macedonia, the former Yugoslav Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4B75AF12-E364-4592-9909-E53EC1A09BCC','Madagascar')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F1427BC0-0F48-442B-8484-68FEA6702D62','Malawi')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('D74A0614-9BA8-4744-AA86-C0BC2E3D9F7B','Malaysia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CCF7E6A4-B776-4E99-9F4E-D35E618F8BDF','Maldives')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('6B2A756A-C6F3-4A11-AB33-088D807B10ED','Mali')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('53E92AEF-3DE7-4BE0-BE0F-900D3C54158C','Malta')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('6C15A18A-DD5B-4AA8-B6F5-FF74F3F902DF','Marshall Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CFBD6784-2274-4FC0-80C5-CE30BB3FE2CD','Martinique')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AD5B96D3-578E-45B4-BF78-435E1B94045D','Mauritania')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('479C8899-219D-4FB1-80CF-D77142598CD6','Mauritius')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8D2966CD-1B7A-4ED1-8BDF-A3C5AB7A6DE1','Mayotte')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('6C27A9DC-5FAC-464A-B35D-B019D9C97AD6','Mexico')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9F4D7A1C-F4BB-4C13-A04C-62AB5B0CE5D4','Micronesia, Federated States of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DAE94DC0-B14B-427A-9730-94F77B38D886','Moldova')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AEC0B33E-F450-466C-972D-35ACB8E28131','Monaco')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E9D49A40-D597-4D53-890D-368BA3E36E13','Mongolia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F1EF4A0E-868D-4E13-B233-FED58694E1B5','Montenegro')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('80D3BA44-F6DA-455F-955A-C70D21204A8E','Montserrat')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B022F766-4BEA-4FF5-A685-CFA849E35428','Morocco')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('769DAC1F-66BF-472E-983E-4E33163CFB75','Mozambique')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4F089FF3-5242-4948-AF10-88CB03377752','Myanmar')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A34FB9A2-8C39-43F0-9D56-A737F839A5DE','Namibia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('1973D433-0741-41E0-BE45-687209F93EB0','Nauru')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('00CAE49C-9E87-426B-9296-F1FD3983EC52','Nepal')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('46A8D2B9-F875-48B0-80A5-C9E733B14365','Netherlands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9C79A5B7-B3CF-4C7E-A55A-7336F6DD195B','New Caledonia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('14534E29-0AF2-424E-973A-AE9F42813862','New Zealand')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('062E2984-A3BF-4424-94E8-318C6EF4B973','Nicaragua')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E9C59E76-6635-4E1B-8132-46DA108A9AC1','Niger')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('6274C893-7282-4961-84DE-C349547B569C','Nigeria')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('FB536349-010D-441E-AADB-697F085D783A','Niue')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('90E538BD-F39E-4C96-8162-647E3D231640','Norfolk Island')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0244CBCE-1951-4555-9E89-60CE9073957A','Northern Mariana Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B5842760-4F32-4253-93E3-DE541D8CBD9E','Norway')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E25B8CC8-8EE4-4E22-9B82-62B7249BDF80','Oman')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('68696DA0-2CB7-4559-B2D7-052074E1EDD1','Pakistan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8D068753-1668-4C38-ADEE-A1EA6AB53199','Palau')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('86A5B651-7E18-4195-A1A4-A0339B42DE6F','Palestine, State of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F7716A81-7643-4C24-9319-F461AD334025','Panama')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('0C77FE7A-7E72-40DC-AD10-DE0BAD5A2A98','Papua New Guinea')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E976714E-A1AE-4669-8C8F-2B7B93ABBC38','Paraguay')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4812280D-1637-4B42-B0B1-D735D3F3EFC4','Peru')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DA846D81-9416-4851-9956-8BA1ED464A65','Philippines')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2C77DCE1-CD59-40CB-B95E-F7209C9DAD4C','Pitcairn')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5B2C5853-19D5-4036-9747-5F2E92C2C450','Poland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DF860DD5-DB05-4458-BA3A-F5631A94F55D','Portugal')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E8751C0C-50D8-4A3E-8505-D1A674769D93','Puerto Rico')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('320D6C8F-36E9-4860-B370-DF76A804D6B6','Qatar')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('52254502-9C8E-4868-833C-889B7A661BD6','Réunion')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('1B68F670-7F2C-4B36-AD85-E8B93114AFAC','Romania')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('BD65DC5E-5378-433E-80FD-2FEDB656EFE4','Russian Federation')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4350BA96-0669-4914-A8B3-33F02C0B8BDF','Rwanda')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5534DED1-7068-430C-967A-A832C1900B52','Saint Barthélemy')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('6BA398E1-3AE1-4B43-BD9D-D8D944BAAD87','Saint Helena, Ascension and Tristan da Cunha')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('750E328A-7A9F-4719-98AC-05AA629E382D','Saint Kitts and Nevis')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('673661C9-6A21-46FF-80A6-DCB172334DF2','Saint Lucia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5CD05424-F380-47E1-A0CC-12EA73C86632','Saint-Martin (France)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('010E65E3-27C9-45BD-9DE8-C566077AC4AF','Saint Pierre and Miquelon')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F7BC3A22-A240-4DD4-AB19-F978C3700018','Saint Vincent and the Grenadines')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5440E147-D989-4468-8927-75BC1493FB19','Samoa')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('49DEE7CC-4DD9-4A8C-8B3F-0D3DCFC0E4F0','San Marino')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('514E4EA0-4704-4068-9E56-31FA492EB6B2','Sao Tome and Principe')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A85AD242-CB6B-4FBD-8754-DDFEC5403614','Saudi Arabia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2FD3BA5E-4C9D-471F-857A-66FD07172F69','Senegal')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7B815054-350D-4604-B83D-389566960B0E','Serbia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2739792D-E2C7-4A3D-B5CA-7C8947D4E9F5','Seychelles')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('54BB5DE0-4476-420B-900D-33568CEF6B45','Sierra Leone')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F51D8BE3-B4A4-486C-BAF7-650A0B52523B','Singapore')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('EA004417-2088-4DA3-9C7E-2C786F1C0AFD','Sint Maarten (Dutch part)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C3AB3FC3-C6CC-41DE-BB63-DE33FC81A3AE','Slovakia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5F91A3F9-5E4D-4687-84B1-4DA81B084444','Slovenia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B8D77FB4-3B71-48E8-BD49-EEAC848155F5','Solomon Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('D217CCBC-6A67-4432-8D3F-B2CCF927D5E9','Somalia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('44314112-7114-438D-B0ED-09FFE7F73655','South Africa')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('35D4E623-9BA4-4430-B541-5218C6B1622C','South Georgia and the South Sandwich Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C7439F4D-D266-43FE-AFC5-C215C42A30CC','South Sudan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('66646128-8521-4A5A-9311-E9E41FF6C616','Spain')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E2D91F16-335F-48CB-9B3D-8F0A3E1DC895','Sri Lanka')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('11F3C6F1-69AC-4B3F-A444-21F46B31000F','Sudan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('78D22FAD-15D1-45DF-9F70-0316D96027D0','Suriname')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('075D366B-74B5-485B-A603-3F5C9BAE2A46','Svalbard and Jan Mayen Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9AE85D79-FAFB-490C-9B69-EB910538B4EE','Swaziland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('ECDF2A74-738B-4D55-B861-00BCDC420CC2','Sweden')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('D9D59B40-0732-4403-8117-1B489E4CA2C2','Switzerland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('3CB932C3-0CD6-4F0B-8580-A26F92A57951','Syria Arab Republic')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('FC79A308-97A3-4C2D-99E2-DB31A3DD1422','Taiwan (Province of China)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9C33ADB0-52F6-41D4-A557-3A5613A30C75','Tajikistan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8CFDAEAC-1566-43E1-AD5F-9042F9ED40D5','Tanzania, United Republic of')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8A774650-6996-40D9-8C00-980E00F2FEDE','Thailand')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('5D0CD1BC-E624-4CA3-89BA-E209586150C9','Timor-Leste')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('A26DC0BC-645B-4AB8-95A0-38291F3CDA18','Togo')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7C5C61A3-9E30-4813-9D7D-E062C02EB1EA','Tokelau')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('E625ED77-189A-4F96-B714-A32743EF21F4','Tonga')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B0279E2D-44E0-44CA-A980-D5BA4B78325A','Trinidad and Tobago')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9FCBFCD5-625A-4FA4-8387-B3178F2C287F','Tunisia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B20FDE5B-688D-4804-B7A3-66F8DFE4EC6C','Turkey')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('AE7B6BD1-AE4D-4181-8256-719576968516','Turkmenistan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('29F33272-C55F-4613-AA4A-AD1AB17654F6','Turks and Caicos Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('2C89491D-33C7-413E-A1F4-62A49E0F7766','Tuvalu')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('C191D9B8-755A-4E2C-898F-6739F1F29AF6','Uganda')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B2BA3EF1-DCCF-4871-B0F5-A83335ACBEE5','Ukraine')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('B811DF78-EAAA-44CB-AA9D-02DA644D6EAD','United Arab Emirates')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('184E1785-26B4-4AE4-80D3-AE319B103ACB','UK - England')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('4209EE95-0882-42F2-9A5D-355B4D89EF30','UK - Scotland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('DB83F5AB-E745-49CF-B2CA-23FE391B67A8','UK - Wales')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('7BFB8717-4226-40F3-BC51-B16FDF42550C','UK - Northern Ireland')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8AEEF0F2-8E72-4ED6-8BFE-719BAA5C462B','United States of America')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('51480C7F-F5CB-4367-B571-61CD45524487','United States Minor Outlying Islands')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('92A1F401-59F0-4261-B734-E59CBDAB1113','Uruguay')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('50F8B792-99CE-41B1-A9A8-F3CC35911B48','Uzbekistan')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('F72AC676-5DDB-4054-85C5-3BF571FEC472','Vanuatu')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('EA42793C-DFB9-4A01-B2E9-D0F4956E4801','Venezuela (Bolivarian Republic of)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('398E7572-D4CD-4FAC-8F59-EB890A7FE58C','Viet Nam')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('CC8D1A0F-33CC-4FF8-A9A5-1A6DBB7DC5CD','Virgin Islands (British)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9A496539-BDB1-4423-9F6B-8AB78AEE002A','Virgin Islands (U.S.)')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('8359B37E-D4C5-449D-8E13-6233F2300DF1','Wallis and Futuna')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('33490CD3-C7EA-4D4C-AD48-BFFBE03F1EE7','Western Sahara')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('9287782C-7C38-46C3-AA68-FA046A8E8087','Yemen')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('65560A6E-8152-4873-8319-63FB2CCEA7D0','Zambia')
INSERT INTO @tempCountryTable([Id],[Name])VALUES('56C5F368-6BE5-4AF9-8434-D5E276740B3C','Zimbabwe')

INSERT INTO [Lookup].[Country]([Id],[Name])
SELECT tmp.[Id], tmp.[Name]
FROM @tempCountryTable tmp
LEFT JOIN [Lookup].[Country] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name]
FROM [Lookup].[Country] LiveTable 
INNER JOIN @tempCountryTable tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO