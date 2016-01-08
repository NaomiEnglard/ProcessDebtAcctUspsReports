Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Configuration

Public Class HtmlParseUtil
    Protected Shared rxTracking As Regex = New Regex(" (?<track>\d+) ")
    Protected Shared rxZip As Regex = New Regex("(?<zip>\d{5})")
    Protected Shared rxStatus As Regex = New Regex("info-text.*"">(?<stat>.+)$")
    Protected Shared rd As System.IO.StreamReader
    Protected Shared wr As System.IO.StreamWriter

    Protected Shared currentTrack As String
    Protected Shared currentDate As String = ""
    Protected Shared currentZip As String = ""
    Protected Shared currentStatus As String = ""
    Protected Shared trackingLogged As Boolean = False


    Protected Shared htmlArchiveDir As String = ConfigurationManager.AppSettings("UspsTrackingDest")

    Protected Shared logFile = "c:\dev\ParsePdf\html.log"

    Public Shared Function getTrackingData(fileName As String) As Boolean

        rd = My.Computer.FileSystem.OpenTextFileReader(fileName)
        wr = My.Computer.FileSystem.OpenTextFileWriter(logFile, True)
        wr.WriteLine("Parsing " & fileName)

        While Not rd.EndOfStream
            Dim line = rd.ReadLine()
            If line.Contains("Tracking Number: ") Then
                line = rd.ReadLine()
                Dim match As Match = rxTracking.Match(line)
                If match.Success Then
                    currentTrack = match.Groups("track").Value

                    getDatesStatusAndZips()
                End If

            End If


        End While
        rd.Close()
        wr.WriteLine("End parsing " & fileName)
        wr.Close()
        Return True   '  TODO Add error checking

    End Function
    Protected Shared Sub getDatesStatusAndZips()
        Dim line As String
        Dim match As Match
        trackingLogged = False

        '  Log the first (which will be the latest) row of tracking data for this tracking number

        While Not rd.EndOfStream
            line = rd.ReadLine()
            If line.Contains("Available Actions") Then
                logDataLine()  'In case we haven't logged it yet
                Return
            End If
            If trackingLogged Then
                Continue While
            End If
            If line.Contains("<td class=""date-time"">") Then
                line = rd.ReadLine()  ' Eat the <p>
                line = rd.ReadLine()
                currentDate = line.Trim()

            End If
            If line.Contains("<td class=""status"">") Then
                While Not rd.EndOfStream
                    line = rd.ReadLine()
                    If line.Contains("</td>") Then Exit While
                    match = rxStatus.Match(line)
                    If match.Success Then
                        line = match.Groups("stat").Value
                        currentStatus = line.Replace("</span>", "").Trim
                        Exit While
                    End If
                End While
            End If
            If line.Contains("<td class=""location"">") Then
                line = rd.ReadLine()  ' Eat the <p>
                line = rd.ReadLine()
                match = rxZip.Match(line)
                If match.Success Then
                    currentZip = match.Groups("zip").Value
                End If
            End If
            logDataLine()   'Log data for this tracking number if it is ready
        End While
    End Sub
    Public Shared Function getDebtAcctId(trackNo As String) As Int32

        '  Stub
        Return 75633
    End Function
    Public Shared Function archiveReport(reportFile As String) As String
        Dim st As String = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")
        Dim newFileName As String = htmlArchiveDir & "\USPSTracking_" & st & ".html"
        Try
            File.Move(reportFile, newFileName)
            Logger.dbg(reportFile & " archived as " & newFileName)

        Catch ex As Exception

            Logger.dbg("Unable to move " & reportFile & " to " & newFileName)
            Logger.dbg(ex.Message)
            Logger.dbg(ex.StackTrace)
            newFileName = ""

        End Try

        Return newFileName

    End Function
    Protected Shared Sub logDataLine()
        If trackingLogged Then Return
        If currentDate <> "" And currentZip <> "" And currentStatus <> "" Then
            '          lg(currentTrack & vbTab & currentDate & vbTab & currentStatus & vbTab & currentZip)
            DataUtil.saveTrackingItem(currentTrack, currentDate, currentStatus, currentZip)

            currentDate = ""
            currentZip = ""
            currentStatus = ""
            trackingLogged = True
        End If
    End Sub
 
    Public Shared Function getUspsTrackingReports() As String()
        ' Returns a list of file names of usps tracking reports to be parsed
        Dim uspsTrackingSourceDir As String = ConfigurationManager.AppSettings("UspsTrackingSrc")
        Dim di As DirectoryInfo = New DirectoryInfo(uspsTrackingSourceDir)
        Dim fiList As FileInfo() = di.GetFiles("*.html")
        Dim nFiles As Int32 = fiList.Length
        Dim fnList As String() = New String(nFiles - 1) {}
        Dim i As Int16

        For i = 0 To nFiles - 1
            fnList(i) = fiList(i).FullName




        Next
        Return fnList

    End Function

End Class

