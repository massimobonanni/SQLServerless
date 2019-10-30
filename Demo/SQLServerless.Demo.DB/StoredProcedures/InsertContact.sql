CREATE PROCEDURE [dbo].[InsertContact]
	@firstName nvarchar(100),
	@lastName nvarchar(100),
	@email nvarchar(255) = null
AS
	insert into dbo.Contacts
		(FirstName,LastName,Email)
	values (@firstName,@lastName,@email)

	RETURN SCOPE_IDENTITY()
