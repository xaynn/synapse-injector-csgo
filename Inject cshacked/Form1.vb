Public Class Form1

    Private TargetProcessHandle As Integer
    Private pfnStartAddr As Integer
    Private pszLibFileRemote As String
    Private TargetBufferSize As Integer

    Public Const PROCESS_VM_READ = &H10
    Public Const TH32CS_SNAPPROCESS = &H2
    Public Const MEM_COMMIT = 4096
    Public Const PAGE_READWRITE = 4
    Public Const PROCESS_CREATE_THREAD = (&H2)
    Public Const PROCESS_VM_OPERATION = (&H8)
    Public Const PROCESS_VM_WRITE = (&H20)
    Dim sDir As String
    Public Declare Function ReadProcessMemory Lib "kernel32" (hProcess As Integer, lpBaseAddress As Integer,
    lpBuffer As String, nSize As Integer,
    ByRef lpNumberOfBytesWritten As Integer) As Integer

    Public Declare Function LoadLibrary Lib "kernel32" Alias "LoadLibraryA" (
     lpLibFileName As String) As Integer

    Public Declare Function VirtualAllocEx Lib "kernel32" (
     hProcess As Integer,
     lpAddress As Integer,
     dwSize As Integer,
     flAllocationType As Integer,
     flProtect As Integer) As Integer

    Public Declare Function WriteProcessMemory Lib "kernel32" (
     hProcess As Integer,
     lpBaseAddress As Integer,
     lpBuffer As String,
     nSize As Integer,
    ByRef lpNumberOfBytesWritten As Integer) As Integer

    Public Declare Function GetProcAddress Lib "kernel32" (
     hModule As Integer, ByVal lpProcName As String) As Integer

    Private Declare Function GetModuleHandle Lib "Kernel32" Alias "GetModuleHandleA" (
     lpModuleName As String) As Integer

    Public Declare Function CreateRemoteThread Lib "kernel32" (
    hProcess As Integer,
     lpThreadAttributes As Integer,
     dwStackSize As Integer,
     lpStartAddress As Integer,
     lpParameter As Integer,
     dwCreationFlags As Integer,
    ByRef lpThreadId As Integer) As Integer

    Public Declare Function OpenProcess Lib "kernel32" (
     dwDesiredAccess As Integer,
     bInheritHandle As Integer,
     dwProcessId As Integer) As Integer

    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (
     lpClassName As String,
     lpWindowName As String) As Integer

    Private Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (
     hObject As Integer) As Integer

    Sub Inject()
        Dim TargetProcess As Process() = Process.GetProcessesByName("csgo")

        Dim name As String = sDir
        TargetProcessHandle = OpenProcess(PROCESS_CREATE_THREAD Or PROCESS_VM_OPERATION Or PROCESS_VM_WRITE, False, TargetProcess(0).Id)
        pszLibFileRemote = name

        pfnStartAddr = GetProcAddress(GetModuleHandle("Kernel32"), "LoadLibraryA")
        TargetBufferSize = 1 + Len(pszLibFileRemote)
        Dim Rtn As Integer
        Dim LoadLibParamAdr As Integer
        LoadLibParamAdr = VirtualAllocEx(TargetProcessHandle, 0, TargetBufferSize, MEM_COMMIT, PAGE_READWRITE)
        Rtn = WriteProcessMemory(TargetProcessHandle, LoadLibParamAdr, pszLibFileRemote, TargetBufferSize, 0)
        CreateRemoteThread(TargetProcessHandle, 0, 0, pfnStartAddr, LoadLibParamAdr, 0, 0)
        CloseHandle(TargetProcessHandle)
        Label3.Text = "Zinjectowano."
        Label3.ForeColor = Color.FromArgb(30, 194, 27)
        If CheckBox1.Checked = True Then
            Application.Exit()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If sDir = "" Then
            MsgBox("Nie wybrales sciezki do DLL.")
        Else
            CheckIfRunning()

        End If
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Dim FileName As String = OpenFileDialog1.FileName.Substring(OpenFileDialog1.FileName.LastIndexOf("\"))
        Dim DllFileName As String = FileName.Replace("\", "")

    End Sub

    Dim p() As Process

    Private Sub CheckIfRunning()
        p = Process.GetProcessesByName("csgo")
        If p.Count > 0 Then
            Inject()
        Else
            MsgBox("Nie znaleziono CS:GO.")
        End If
    End Sub


    Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        OpenFileDialog1.Filter = "DLL (*.dll) |*.dll"
        OpenFileDialog1.ShowDialog()
        sDir = IO.Path.GetDirectoryName(OpenFileDialog1.FileName.ToString + "/")
        If sDir = "OpenFileDialog1" Then
            MsgBox("Nie wybrales sciezki do DLL.")
            sDir = ""
        Else
            Label3.Text = "Wybrano sciezke, gotowe do wstrzykniecia."
            Label3.ForeColor = Color.FromArgb(30, 194, 27)
        End If

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label3.Text = "Nie wybrano sciezki DLL."
        Label3.ForeColor = Color.FromArgb(255, 0, 0)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Application.Exit()
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
End Class
