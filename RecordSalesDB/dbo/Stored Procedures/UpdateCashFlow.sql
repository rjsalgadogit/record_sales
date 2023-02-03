-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCashFlow]
	@Id int,
	@Code nvarchar(128),
	@TransactionTypeId int,
	@Amount decimal(18,2),
	@Description nvarchar(128),
	@Notes nvarchar(MAX),
	@User nvarchar(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM CashFlow Where Id = @Id)
	BEGIN
		UPDATE CashFlow
		SET TransactionTypeId = @TransactionTypeId,
			Amount = @Amount,
			Description = @Description,
			Notes = @Notes,
			ModifiedUser = @User,
			ModifiedDate = GETUTCDATE()
		WHERE Id = @Id
	END
	ELSE
	BEGIN
		INSERT INTO CashFlow (Code, TransactionTypeId, Amount, Description, Notes, CreationUser, CreationDate)
		SELECT @Code, @TransactionTypeId, @Amount, @Description, @Notes, @User, GETUTCDATE()
	END
END