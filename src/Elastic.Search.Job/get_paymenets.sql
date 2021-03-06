DECLARE @startRow INT = {0}
DECLARE @endRow INT = {1};

SELECT
	*
FROM
(
	SELECT
		ROW_NUMBER() OVER (ORDER BY p.PAYMENT_ID ASC) as RowNumber,
		CAST(p.PAYMENT_ID AS VARCHAR(MAX)) Confirmation,
		LTRIM(RTRIM(p.CREDIT_ACCOUNT)) CreditAccount,
		LTRIM(RTRIM(p.CREDIT_FIRSTNAME)) FirstName,
		LTRIM(RTRIM(p.CREDIT_LASTNAME)) LastName,                       
		CASE 
			WHEN LEN(RTRIM(LTRIM(p.DEBIT_ACCOUNT))) <= 4 THEN '*' + RIGHT(RTRIM(LTRIM(p.DEBIT_ACCOUNT)), 1)  
			ELSE '*' + RIGHT(RTRIM(LTRIM(p.DEBIT_ACCOUNT)), 4) 
		END AS DebitAccountMask,
		LTRIM(RTRIM(p.DEBIT_ACCOUNT)) DebitAccount,
		p.FEE_AMOUNT AS FeeAmount,
		CASE 
			WHEN [prc].[STATUS] IS NULL AND p.[RESULT_CODE] IS NOT NULL THEN p.[RESULT_CODE]
			WHEN [prc].[STATUS] IS NULL AND p.[PAYMENT_ACTUATED_DATE] IS NULL AND p.[EXPORT_DATE] IS NOT NULL THEN 'Exported'
			WHEN [prc].[STATUS] IS NULL AND p.[PAYMENT_ACTUATED_DATE] IS NULL AND p.[EXPORT_DATE] IS NULL AND p.[TRANSTYPE] IN ('E', 'N', 'T') THEN 'Refund - Pending'
			WHEN [prc].[STATUS] IS NULL AND p.[PAYMENT_ACTUATED_DATE] IS NULL AND p.[EXPORT_DATE] IS NULL THEN 'Pending'
			WHEN [prc].[STATUS] IS NULL AND p.[PAYMENT_ACTUATED_DATE] IS NOT NULL AND p.[TRANSTYPE] IN ('E', 'N', 'T') THEN 'Refund - Processed'	            		            
			WHEN [prc].[STATUS] IS NULL AND p.[PAYMENT_ACTUATED_DATE] IS NOT NULL THEN 'Processed'
			ELSE [prc].[STATUS]
		END AS Status,
		ISNULL(NULLIF(LTRIM(RTRIM(p.EMAIL1)), ''), NULL) Email,
		LTRIM(RTRIM(p.PAYMENT_ENTERED_BY)) AS Channel,
		p.PAYMENT_AMOUNT Amount,
		p.PAYMENT_ENTERED_DATE EnteredDate,
		p.PAYMENT_ACTUATED_DATE ActuatedDate,
		p.SCHEDULED_PAYMENT_DATE ScheduledDate,
		CASE 		            
			WHEN p.[PAYMENT_TYPE] = 'C' THEN 		            
			(							
				SELECT REPLACE(REPLACE([dbo].[GetCardType_v1]([DEBIT_ACCOUNT]), 'UNKNOWN'	, 'Other'), 'AMEX', 'AmericanExpress')												
			)	           
			WHEN p.[PAYMENT_TYPE] = 'T' THEN 'ATM'		            
			WHEN p.[PAYMENT_TYPE] = 'A' THEN 'ACH'         
			ELSE ''	            
		END AS CardIssuer,
		p.[PAYMENT_TYPE] PaymentType,
		CASE 
			WHEN (p.IS_FAILED_PAYMENT = 1) 
					OR (p.[PAYMENT_ACTUATED_DATE] IS NOT NULL AND [prc].[NEGATIVE] = 1) 
					OR ([prc].[STATUS] IS NULL AND p.[RESULT_CODE] IS NOT NULL) 
					THEN 'red'
			WHEN p.[PAYMENT_ACTUATED_DATE] IS NOT NULL AND [prc].[NEGATIVE] = 0 THEN 'black'
			WHEN p.[PAYMENT_ACTUATED_DATE] IS NULL THEN 'orange'
			ELSE ''
		END Color,
		ISNULL(NULLIF(LTRIM(RTRIM(REPLACE(p.PHONE, '-', ''))), ''), NULL)  PhoneOne,
		ISNULL(NULLIF(LTRIM(RTRIM(REPLACE(p.PHONE2, '-', ''))), ''), NULL) PhoneTwo,
		HAS_AUDIT_DETAILS HasAuditDetails,
		LTRIM(RTRIM(p.COLLECTOR_ID)) CollectorId,
		LTRIM(RTRIM(p.CREDIT_SITE_ID)) CreditSiteId
	FROM 
		(   
			SELECT  p.PAYMENT_ID, p.CREDIT_ACCOUNT, p.CREDIT_FIRSTNAME, p.CREDIT_LASTNAME, p.FEE_AMOUNT, p.PAYMENT_ENTERED_BY, p.PAYMENT_AMOUNT, p.PAYMENT_ENTERED_DATE, p.PAYMENT_TYPE, 
					p.DEBIT_ACCOUNT, p.PAYMENT_ACTUATED_DATE, p.PHONE, p.PHONE2, p.TRANSTYPE, p.RESULT_CODE, p.SCHEDULED_PAYMENT_DATE, p.COLLECTOR_ID, p.CREDIT_SITE_ID, p.EXPORT_DATE, 
					1 AS HAS_AUDIT_DETAILS, px.EMAIL1, 0 as IS_FAILED_PAYMENT
			FROM [dbo].[PAYMENTS] [p] WITH(NOLOCK)
			LEFT JOIN [dbo].[PAYMENTS_EXTENSION] [px] WITH(NOLOCK) ON [px].[PAYMENT_ID] = [p].[PAYMENT_ID]	
		)p
		LEFT JOIN PAYMENTRESULTCODES prc
			ON prc.CODE = p.RESULT_CODE 
				AND prc.PAYMENT_TYPE = p.PAYMENT_TYPE
	WHERE
		p.PAYMENT_ID > {2}
) t
WHERE
	RowNumber > @startRow AND RowNumber <= @endRow