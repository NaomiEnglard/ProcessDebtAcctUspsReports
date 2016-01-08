Imports System.IO

Module Module1
  
    Sub Main(args() As String)

   

  
                readUspsReports()
   


    End Sub

   
    Sub readUspsReports()
        Logger.msg("Reading  USPS Reports")
        Dim files As String() = HtmlParseUtil.getUspsTrackingReports()
        Dim nFiles As Int32 = files.Count
        Logger.msg("Number of files " & CStr(nFiles))

        For Each f As String In files
            If (HtmlParseUtil.getTrackingData(f)) Then
                Logger.dbg(f & " parsed.  About to archive")
                Dim newName As String = HtmlParseUtil.archiveReport(f)
            Else
                Logger.err("Unable to parse " & f)
            End If
            Threading.Thread.Sleep(5)  'Make sure we don't have duplicate archive names

        Next
    End Sub


End Module
