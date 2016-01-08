Imports System.Data.SqlClient
Imports System.Configuration

Public Class DataUtil
  

    Public Shared Function saveTrackingItem(trackingNumber As String, dt As String, status As String, zip As String) As Boolean

        Logger.dbg("Inserting dispute tracking record " & bracket(trackingNumber) & bracket(dt) & bracket(status) & bracket(zip))


        Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("frodo").ConnectionString)
        conn.Open()


        Dim cmd As SqlCommand = New SqlCommand("AddDisputeTrackingLogEntry", conn)
        Dim trackDate As Date

        Dim dateParsed As Boolean = Date.TryParse(dt, trackDate)
        Dim paramDate As SqlParameter = New SqlParameter()

        If dateParsed Then
            paramDate.Value = trackDate
        Else
            paramDate.Value = DBNull.Value
        End If

        cmd.CommandType = CommandType.StoredProcedure


        With cmd.Parameters
            .Add("@TrackingNumber", SqlDbType.VarChar).Value = trackingNumber
            .Add("@TrackingDate", SqlDbType.Date).Value = paramDate.Value
            .Add("@TrackingStatus", SqlDbType.VarChar).Value = status
            .Add("@TrackingZip", SqlDbType.VarChar).Value = zip
        End With

        Try
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            Logger.err("Unable to insert record for tracking number " & trackingNumber)
            Logger.err(ex.Message)
            Logger.err(ex.StackTrace)
            Return False
        Finally
            conn.Close()

        End Try

        Return True

    End Function

    Public Shared Function addDisputeTrackingDebtAccount(trackingNumber As String, debtAccountId As Int32)
        Logger.dbg("addDisputeTrackingDebtAccount " & bracket(trackingNumber) & bracket(CStr(debtAccountId)))
        Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("frodo").ConnectionString)
        conn.Open()


        Dim cmd As SqlCommand = New SqlCommand("AddDisputeTrackingDebtAccount", conn)
        cmd.CommandType = CommandType.StoredProcedure
        With cmd.Parameters
            .Add("@TrackingNumber", SqlDbType.VarChar).Value = trackingNumber
            .Add("@DebtAccountId", SqlDbType.Int).Value = debtAccountId

        End With

        Try
            cmd.ExecuteNonQuery()
            Return True

        Catch ex As Exception
            Logger.err("Unable to add record " & bracket(trackingNumber) & bracket(CStr(debtAccountId)))
            Logger.err(ex.Message)
            Logger.err(ex.StackTrace)
            Return False

        Finally
            conn.Close()

        End Try

        Return True

    End Function


    Private Shared Function bracket(s As String) As String
        Return "[" & s & "] "

    End Function
End Class
