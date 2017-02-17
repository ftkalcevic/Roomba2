CREATE PROCEDURE [dbo].[UpdateProperty]
	@Properties dbo.PropertiesTableType READONLY
AS
BEGIN
    MERGE INTO dbo.Properties AS T
    USING @Properties AS S
    ON T.Name = S.Name
    WHEN MATCHED THEN UPDATE SET T.Value = S.Value
    WHEN NOT MATCHED THEN INSERT (Name,Value) VALUES(S.Name,S.Value);
END
