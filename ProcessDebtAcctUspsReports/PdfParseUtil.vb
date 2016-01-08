Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Configuration

Public Class PdfParseUtil
    Public Shared rxTrack As Regex = New Regex("(?<track>\d{4} \d{4} \d{4} \d{4})")
    Public Shared rxAcct As Regex = New Regex("(?<acct>CA\d+) ")
    Protected Shared disputeLettersSourceDir As String = ConfigurationManager.AppSettings("DisputeLettersSrc")
    Protected Shared disputeLettersDestDir As String = ConfigurationManager.AppSettings("DisputeLettersDesc")
    Protected Shared deadLetterBox As String = ConfigurationManager.AppSettings("DisputeLettersNonAccountDest")




    Public Shared Function getTrackingNumberAndAccount(fileName As String, ByRef tracking As String, ByRef acct As String) As String
        Dim pdfReader As PdfReader = New PdfReader(fileName)
        Dim strategy As ITextExtractionStrategy = New SimpleTextExtractionStrategy()
        Dim errMsg As String = ""


        Dim text As String = PdfTextExtractor.GetTextFromPage(pdfReader, 1, strategy)

        Dim match As Match = rxTrack.Match(text)
        tracking = ""
        acct = ""
        If Not match.Success Then
            errMsg = "Tracking number not found"
        Else
            tracking = match.Groups("track").Value
        End If
        match = rxAcct.Match(text)
        If Not match.Success Then
            If errMsg Then errMsg = errMsg & vbCrLf

            errMsg = errMsg & "Debt account not found"
        Else
            acct = match.Groups("acct").Value
        End If

        Return errMsg

    End Function

    Public Shared Function getDisputeLetters() As FileInfo()
        ' Returns a list FileInfo objects describing the files to be read

        Dim di As DirectoryInfo = New DirectoryInfo(disputeLettersSourceDir)
        Dim fiList As FileInfo() = di.GetFiles("*.pdf")
      
        Return fiList

    End Function

    'fullName is the  fully qwualified name of the pdf file
    ' name is the unqualified name of the pdf file
    Public Shared Function archiveDisputeLetter(fullName As String, name As String, acctNumber As Int32) As String
        Dim di As DirectoryInfo = New DirectoryInfo(disputeLettersDestDir)
        Dim debtAcctDirPath As String = disputeLettersDestDir & "\" & CStr(acctNumber)
        Dim retMsg As String = ""
        If Not Directory.Exists(debtAcctDirPath) Then
            '  Try to create the directory
            Dim acctDi As DirectoryInfo

            Try
                acctDi = Directory.CreateDirectory(debtAcctDirPath)

            Catch ex As Exception
                retMsg = "Unable to create debt account directory " & debtAcctDirPath
                Logger.err(retMsg)
                Logger.err(ex.Message)
                Logger.err(ex.StackTrace)
                Return retMsg
            End Try
        End If
        Dim destQualifiedFileName = debtAcctDirPath & "\" & name
        Try
            File.Move(fullName, destQualifiedFileName)
        Catch ex As Exception
            retMsg = "Unable to move " * fullName & " to " & destQualifiedFileName
            Logger.err(retMsg)
            Logger.err(ex.Message)
            Logger.err(ex.StackTrace)
            Return retMsg
        End Try


        Return ""

    End Function

    Public Shared Function sendNonOcr(qualifiedFileName As String, fileName As String) As String
        Dim qualifiedDestFile As String = deadLetterBox & "\" & fileName
        Try
            File.Move(qualifiedFileName, qualifiedDestFile)
            Return ""

        Catch ex As Exception
            Dim retMsg As String = "Unable to archive non-acct letter " & qualifiedFileName & " to " & qualifiedDestFile
            Logger.err(retMsg)
            Logger.err(ex.Message)
            Logger.err(ex.StackTrace)
            Return retMsg
        End Try

    End Function

End Class

