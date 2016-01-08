USE [WSMProcessing]
GO

/****** Object:  StoredProcedure [dbo].[AddDisputeTrackingLogEntry]    Script Date: 1/6/2016 2:04:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*  If an entry with this tracking number exists, fill in the date, status and zip, 
    otherwise create a new entry
*/

CREATE PROCEDURE [dbo].[AddDisputeTrackingLogEntry]

	@TrackingNumber varchar(30),
	@TrackingDate Date,
	@TrackingStatus varchar(50),
	@TrackingZip varchar(20)



AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF EXISTS(SELECT TrackingNumber from DisputeTrackingLog where TrackingNumber = @TrackingNumber)
		BEGIN
			
			UPDATE DisputeTrackingLog 
			SET
			TrackingDate = @TrackingDate,
			TrackingStatus = @TrackingStatus,
			TrackingZip = @TrackingZip
			WHERE TrackingNumber = @TrackingNumber
		END
	ELSE
		BEGIN
		
		INSERT INTO DisputeTrackingLog (TrackingNumber, TrackingDate, TrackingStatus, TrackingZip) VALUES
			(@TrackingNumber, @TrackingDate, @TrackingStatus, @TrackingZip)
	END
    


END



GO


