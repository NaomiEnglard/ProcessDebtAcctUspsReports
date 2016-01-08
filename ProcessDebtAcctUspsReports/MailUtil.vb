Imports System.Net.Mail
Imports System.IO
Imports System.Configuration

Public Class MailUtil
    Protected Shared testMailTo As String = "elliot@websourcemarket.com"
    Protected Shared testMailFromSt As String = "elliot.katz@gmail.com"

    Protected Shared testMailFrom = New MailAddress(testMailFromSt)
    Protected Shared client As SmtpClient = Nothing

    Protected Shared Sub configureClient()
        If Not IsNothing(client) Then Return
        client = New SmtpClient()
        client.EnableSsl = True





    End Sub

    ' 

    Public Shared Sub sendProblemFile(qualifiedFileName As String, errMsg As String)


        configureClient()
        Dim msgMail As New MailMessage
        msgMail.To.Add(testMailTo)
        msgMail.From = testMailFrom


        msgMail.Subject = "Unable to process dispute letter"
        msgMail.Body = errMsg
        Dim stream As FileStream = Nothing
        Dim attachment As Attachment
        Try
            stream = File.OpenRead(qualifiedFileName)
            attachment = New Attachment(stream, qualifiedFileName)
            msgMail.Attachments.Add(attachment)
            client.Send(msgMail)

        Catch ex As Exception
            Logger.err("Unable to send exception email for file " & qualifiedFileName)
            Logger.err(ex.Message)
            Logger.err(ex.StackTrace)
        Finally
            If Not IsNothing(stream) Then stream.Dispose()

        End Try

    End Sub
End Class
