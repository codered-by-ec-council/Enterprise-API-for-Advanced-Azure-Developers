/****** Object:  Table [dbo].[Address]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Address1] [nvarchar](200) NULL,
	[Address2] [nvarchar](200) NULL,
	[Address3] [nvarchar](200) NULL,
	[Town] [nvarchar](200) NULL,
	[Lockstamp] [timestamp] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Age] [int] NULL,
	[Email] [nvarchar](200) NULL,
	[Lockstamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[AddCustomer]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[AddCustomer]
(
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Age int,
	@Email nvarchar(200),
	@Address1 nvarchar(200),
	@Address2 nvarchar(200),
	@Address3 nvarchar(200),
	@Town nvarchar(200),
	@CustomerId_out int output
)
AS
BEGIN
    
	-- add the customer 
    insert into [dbo].[Customer] ([FirstName],[LastName],[Age],[Email])
	values (@FirstName,@LastName,@Age, @Email)

	set @CustomerId_out = @@IDENTITY

	--add the address
	insert into [dbo].[Address] ([CustomerId],[Address1],[Address2],[Address3],[Town])
	values ( @CustomerId_out,@Address1, @Address2, @Address3,@Town)

    
END
GO
/****** Object:  StoredProcedure [dbo].[DelCustomerById]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[DelCustomerById]
(
	@CustomerId int,
	@LockStamp bigint
)
AS
BEGIN
    
		--check if record exists
	if not exists ( select 1 from dbo.Customer where  CustomerId = @CustomerId )
		return 1;

	--check if lockstamps are same
	if not exists ( select 1 from dbo.Customer where  CustomerId = @CustomerId and CONVERT(bigint,Lockstamp) = @LockStamp )
		return 2;
	
    Delete [dbo].[Customer] where CustomerId = @CustomerId and  CONVERT(bigint,[Lockstamp]) = @LockStamp
	Delete [dbo].[Address] where CustomerId = @CustomerId

	return 0
			
END
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerById]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[GetCustomerById]

 @CustomerId int
 
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT C.[CustomerID], C.[FirstName], C.[LastName],C.[Email], CONVERT(bigint,C.[Lockstamp]) as Lockstamp,
	A.AddressId, A.Address1, A.Address2, A.Address3, A.Town
	from [Customer] C left join [dbo].[Address] A on C.CustomerId = A.CustomerId
	where C.CustomerId = @CustomerId

END
GO
/****** Object:  StoredProcedure [dbo].[GetCustomers]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[GetCustomers]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT C.[CustomerID], C.[FirstName], C.[LastName], C.[Email], CONVERT(bigint,C.[Lockstamp]) as Lockstamp,
	A.AddressId, A.Address1, A.Address2, A.Address3, A.Town
	from [Customer] C left join [dbo].[Address] A on C.CustomerId = A.CustomerId

END
GO
/****** Object:  StoredProcedure [dbo].[UpdCustomer]    Script Date: 24/10/2021 3:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[UpdCustomer]
(
	@CustomerId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Age int,
	@Email nvarchar(200),	
	@Address1 nvarchar(200),
	@Address2 nvarchar(200),
	@Address3 nvarchar(200),
	@Town nvarchar(200),
	@LockStamp bigint
)
AS
BEGIN
    

	--check if record exists
	if not exists ( select 1 from dbo.Customer where  CustomerId = @CustomerId )
		return 1;

	--check if lockstamps are same
	if not exists ( select 1 from dbo.Customer where  CustomerId = @CustomerId and CONVERT(bigint,Lockstamp) = @LockStamp )
		return 2;
	
	-- update the customer 
	Update dbo.Customer
	set FirstName = @FirstName,
		LastName = @LastName,
		Age = @Age,
		Email = @Email
		
	where CustomerId = @CustomerId and CONVERT(bigint,Lockstamp) = @LockStamp

	if(@@ROWCOUNT = 1)
	begin
		-- update address
		Update dbo.[Address]
		set Address1 = @Address1,
			Address2 = @Address2,
			Address3 = @Address3,
			Town = @Town
		where CustomerId = @CustomerId 
	end
	
	return 0
END
GO
