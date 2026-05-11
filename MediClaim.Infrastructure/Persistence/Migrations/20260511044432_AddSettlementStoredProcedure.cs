using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClaim.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlementStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
CREATE PROCEDURE usp_SettleClaim
    @ClaimId UNIQUEIDENTIFIER,
    @ApprovedAmount DECIMAL(18,2),
    @PolicyId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRANSACTION;

    BEGIN TRY

        -- Idempotency check

        IF EXISTS
        (
            SELECT 1
            FROM Claims
            WHERE ClaimId = @ClaimId
            AND Status = 8
        )
        BEGIN
            SELECT 2 AS ResultCode;

            COMMIT TRANSACTION;

            RETURN;
        END

        DECLARE @AnnualLimit DECIMAL(18,2);
        DECLARE @UsedAmount DECIMAL(18,2);

        SELECT
            @AnnualLimit = AnnualLimit,
            @UsedAmount = UsedAmount
        FROM Policies WITH (UPDLOCK, ROWLOCK)
        WHERE PolicyId = @PolicyId;

        IF (@UsedAmount + @ApprovedAmount)
            > @AnnualLimit
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT 1 AS ResultCode;

            RETURN;
        END

        UPDATE Policies
        SET UsedAmount =
            UsedAmount + @ApprovedAmount
        WHERE PolicyId = @PolicyId;

        UPDATE Claims
        SET Status = 8,
            ApprovedAmount = @ApprovedAmount
        WHERE ClaimId = @ClaimId;

        COMMIT TRANSACTION;

        SELECT 0 AS ResultCode;

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
