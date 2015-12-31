-- dbo schema
-- select and update permissions
GRANT SELECT, UPDATE ON SCHEMA::[dbo] TO [weee_application]

--Lookup schema
-- select permissions
GRANT SELECT ON SCHEMA::[Lookup] TO [weee_application]

--Auditing schema
-- select and insert permissions
GRANT SELECT, INSERT ON SCHEMA::[Auditing] TO [weee_application]

---select, insert, update and delete permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Identity] TO [weee_application]
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Admin] TO [weee_application]
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Organisation] TO [weee_application]
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[PCS] TO [weee_application]
GRANT EXECUTE, SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Charging] TO [weee_application]
GRANT EXECUTE, SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[Producer] TO [weee_application]

GO