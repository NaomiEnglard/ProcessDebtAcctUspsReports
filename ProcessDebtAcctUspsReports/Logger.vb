Imports System.Configuration
Imports System.Data.SqlClient

Public Class Logger


    Private Shared LEVEL_DEBUG As String = "debug"
    Private Shared LEVEL_MESSAGE As String = "message"
    Private Shared LEVEL_ERROR As String = "error"
    Private Shared LEVEL_NONE As String = "none"
    Private Shared logLevel As String = ""

    Private Shared Function getLogLevel()
        If logLevel <> "" Then Return logLevel
        logLevel = LEVEL_DEBUG
        Return logLevel
    End Function

    Private Shared Function getCurrentLog() As String
        Dim logFileName As String = ConfigurationManager.AppSettings("LogFile")
        logLevel = ConfigurationManager.AppSettings("LogLevel")

        If logFileName Is Nothing Then

            logLevel = LEVEL_NONE
            Return ""
        End If
        logFileName = logFileName & "." & Date.Now.ToString("yyyy.MM.dd") & ".log"
        Return logFileName
    End Function

    Private Shared Function isDebug() As Boolean
        Return getLogLevel() = LEVEL_DEBUG
    End Function
    Private Shared Function isMessage() As Boolean
        Return isDebug() Or (getLogLevel() = LEVEL_MESSAGE)

    End Function
    Private Shared Function isError() As Boolean
        Return isMessage() Or (getLogLevel() = LEVEL_ERROR)
    End Function

    Public Shared Sub dbg(s As String)
        If Not isDebug() Then Return
        Dim logFileName As String = getCurrentLog()
        If logFileName = "" Then Return

        Dim sw As System.IO.StreamWriter
        sw = My.Computer.FileSystem.OpenTextFileWriter(logFileName, True)
        sw.WriteLine(CStr(DateTime.Now) & " " & s)
        sw.Close()

    End Sub


    Public Shared Sub msg(s As String)
        If Not isMessage() Then Return
        Dim logFileName As String = getCurrentLog()
        If logFileName = "" Then Return

        Dim sw As System.IO.StreamWriter
        sw = My.Computer.FileSystem.OpenTextFileWriter(logFileName, True)
        sw.WriteLine(CStr(DateTime.Now) & " " & s)
        sw.Close()

    End Sub

    Public Shared Sub err(s As String)
        If Not isError() Then Return
        Dim logFileName As String = getCurrentLog()
        If logFileName = "" Then Return


        Dim sw As System.IO.StreamWriter
        sw = My.Computer.FileSystem.OpenTextFileWriter(logFileName, True)
        sw.WriteLine(CStr(DateTime.Now) & " ERROR: " & s)
        sw.Close()

    End Sub




End Class
