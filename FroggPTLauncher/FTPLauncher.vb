Imports System.IO
Imports System.Net

' TODO LIST
' Resume version download

Public Class FroggPTLauncher


#Region "Default values"
    'create a thread
    Private Shared cThread
    '############Network
    Private ReadOnly webUrl = "http://www.frogg.fr"
    Private ReadOnly forumUrl = "http://forum.frogg.fr"
    Private ReadOnly facebookUrl = "http://facebook.frogg.fr"
    '############Files
    'first update version
    Public ReadOnly firstVersion = "5160"
    Public Shared clientVersion As String
    'game file
    Private ReadOnly gameFile = "froggpt.exe"
    'game version server file
    Private ReadOnly versionServer = "http://version.frogg.fr/" '0
    Private ReadOnly versionMirrorServer = "http://mirror.version.frogg.fr/" '1
    Private ReadOnly versionParam = "?version="
    Private ReadOnly resumeVersion = "resumeVersion.log"
    'Configuration file
    Private ReadOnly configFile = "ptReg.rgx"
    'game news server url
    Private ReadOnly newsServer = "http://news.frogg.fr/"
    '############Configuration
    Private ReadOnly gameName = "FroggPT"
    Private ReadOnly gameUrl = "game.frogg.fr"
    'check if data are loaded
    Private Shared isLoaded = False
    'Default values
    Private Shared ColorBPP = "32"
    Private Shared WindowMode = "1"
    Private Shared Graphic = "3"
    Private Shared MotionBlur = "true"
    Private Shared ScreenSize = "1"
    'custom values
    Private Shared CustomCharacterSelect = "0"
    Private Shared CustomInterface = "0"
    Private Shared CustomOpening = "0"
    Private Shared CustomSound = "0"
    'version server
    Private Shared ServerVersion = "0"
    'screen values
    Private Shared st = New Dictionary(Of String, String())
    Private Shared sw = New Dictionary(Of String, String)
    Private ReadOnly resMax = 2 'nb max res for window mode
    'init screen values
    Private Sub initValues()
        'Init type values
        st("4:3") = {"800x600", "1024x768", "1280x1024", "1600x1200"}
        st("16:9") = {"800x450", "1024x576", "1280x720", "1600x900"}
        st("16:10") = {"800x500", "1024x640", "1280x800", "1600x1000"}
        'Init screen values
        sw("800x600") = "1"
        sw("1024x768") = "2"
        sw("1280x1024") = "3"
        sw("1600x1200") = "4"
        sw("800x450") = "5"
        sw("1024x576") = "6"
        sw("1280x720") = "7"
        sw("1600x900") = "8"
        sw("800x500") = "9"
        sw("1024x640") = "10"
        sw("1280x800") = "11"
        sw("1600x1000") = "12"
    End Sub
    'Sounds
    Private ReadOnly sndFold = {"wav", "image/Sinimage/sound"}
    Private ReadOnly sndFile = {"StartImage\Opening\intro.wav", "StartImage\login\CharacterSelect.wav"}
    Private ReadOnly sndMove = "z"
#End Region


#Region "Form values"

    'set values in form from loaded values
    Private Sub setFormValues()
        Dim resolution = getKeyFromDictionary(sw, ScreenSize), isFound = False, selectedType = ""
        'Motion Blur
        If MotionBlur = "true" Then OptMotion1.Checked = True
        If MotionBlur = "false" Then OptMotion2.Checked = True
        'Color
        If ColorBPP = "16" Then OptColor1.Checked = True
        If ColorBPP = "32" Then OptColor2.Checked = True
        'window mode
        If WindowMode = "0" Then OptFullScreen.Checked = True
        'Graphic
        If Graphic = "1" Then OptGraphic1.Checked = True
        If Graphic = "2" Then OptGraphic2.Checked = True
        If Graphic = "3" Then OptGraphic3.Checked = True
        'sound test
        If Not My.Computer.FileSystem.DirectoryExists(sndFold(0)) Then OptNoSound.Checked = True
        'Screen Size
        For Each kv As KeyValuePair(Of String, String()) In st
            'Display Key and Value.
            OptScreenT.Items.Add(kv.Key)
            'if key is resolution then mean it is selected
            If Not isFound Then
                isFound = setScreenSList(kv.Value, resolution)
                selectedType = kv.Key
            End If
        Next
        'default selected values
        OptScreenT.SelectedItem = selectedType
        OptScreenS.SelectedItem = resolution
        'check resolution validity
        checkResolution()
    End Sub

    'set values from form values
    Private Sub getFormValues()
        'Motion Blur
        If OptMotion1.Checked = True Then MotionBlur = "true"
        If OptMotion2.Checked = True Then MotionBlur = "false"
        'Color
        If OptColor1.Checked = True Then ColorBPP = "16"
        If OptColor2.Checked = True Then ColorBPP = "32"
        'Graphic
        If OptGraphic1.Checked = True Then Graphic = "1"
        If OptGraphic2.Checked = True Then Graphic = "2"
        If OptGraphic3.Checked = True Then Graphic = "3"
        'window mode
        If OptFullScreen.Checked = True Then WindowMode = "0" Else WindowMode = "1"
        'Screen Size
        ScreenSize = sw(OptScreenS.SelectedItem)
    End Sub

#End Region


#Region "File values"

    'set values to config file
    Private Sub setValues()
        Dim lines As New ArrayList, move = "", remove = "", sndOn = ""
        'sound management
        If OptNoSound.Checked = True Then
            remove = "z"
            sndOn = "Off"
        Else
            move = "z"
            sndOn = "On"
        End If
        'remane soud files & folders
        moveSoundFile(move, remove)
        'create line array
        lines.Add("# /!\ Do Not Change This File By Yourself /!\")
        lines.Add("")
        lines.Add("#Options")
        lines.Add("""Sound"" """ & sndOn & """")
        'set values
        lines.Add("""ColorBPP"" """ & ColorBPP & """")
        lines.Add("""Graphic"" """ & Graphic & """")
        lines.Add("""MotionBlur"" """ & MotionBlur & """")
        lines.Add("""ScreenSize"" """ & ScreenSize & """")
        lines.Add("""WindowMode"" """ & WindowMode & """")
        lines.Add("#Special")
        'not modifiable values
        lines.Add("""Network"" ""3""")
        lines.Add("""CameraInvert"" ""false""")
        lines.Add("""CameraSight"" ""Off""")
        lines.Add("""RainMode"" ""Off""")
        'custom values
        lines.Add("#Custom")
        lines.Add("""CustomCharacterSelect"" """ & CustomCharacterSelect & """")
        lines.Add("""CustomInterface"" """ & CustomInterface & """")
        lines.Add("""CustomOpening"" """ & CustomOpening & """")
        lines.Add("""CustomSound"" """ & CustomSound & """")
        'server values
        lines.Add("#Server")
        lines.Add("""ServerName"" """ & gameName & """")
        lines.Add("""Version"" """ & clientVersionInt & """")
        lines.Add("""Server1"" """ & gameUrl & """")
        lines.Add("""Server2"" """ & gameUrl & """")
        lines.Add("""Server3"" """ & gameUrl & """")
        'server version
        lines.Add("#Version Server")
        lines.Add("""ServerVersion"" """ & ServerVersion & """")
        'create conf file
        writeFile(configFile, lines)
    End Sub


    'get values from config file
    Private Sub getValues()
        Dim config As String, configLine As String(), lineVals As String()
        'check if conf file exist else cannot read info in it
        If My.Computer.FileSystem.FileExists(configFile) Then
            'read file config content
            config = My.Computer.FileSystem.ReadAllText(configFile)
            'get each line
            configLine = config.Split(vbCrLf)
            'for each lines
            For Each line In configLine
                'If Contains char "
                If line.Contains("""") Then
                    'spit char "
                    lineVals = line.Split("""")
                    'if array has at least 3 elements then set values
                    If lineVals.Length > 3 Then
                        Select Case lineVals(1)
                            Case "MotionBlur"
                                MotionBlur = lineVals(3)
                            Case "ColorBPP"
                                ColorBPP = lineVals(3)
                            Case "WindowMode"
                                WindowMode = lineVals(3)
                            Case "Graphic"
                                Graphic = lineVals(3)
                            Case "ScreenSize"
                                ScreenSize = lineVals(3)
                            Case "Version"
                                clientVersion = lineVals(3)
                            Case "CustomCharacterSelect"
                                CustomCharacterSelect = lineVals(3)
                            Case "CustomInterface"
                                CustomInterface = lineVals(3)
                            Case "CustomOpening"
                                CustomOpening = lineVals(3)
                            Case "CustomSound"
                                CustomSound = lineVals(3)
                            Case "ServerVersion"
                                ServerVersion = lineVals(3)
                        End Select
                    End If
                End If
            Next
        End If
    End Sub

#End Region


#Region "Menu Actions"

    Private Sub MenuBgChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuBgChange.Click
        setRdmBg()
    End Sub

    Private Sub MenuHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuHelp.Click
        froggLogo.Show()
    End Sub

    Private Sub MenuOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuOptions.Click
        If MenuOptions.Text = "[ Launcher ]" Then showLauncher() Else showOption()
    End Sub

    Private Sub MenuCustomize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuCustomize.Click
        If MenuCustomize.Text = "[ Launcher ]" Then showLauncher() Else showCustom()
    End Sub

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuExit.Click
        exitMain()
    End Sub

    Private Sub MenuStartGame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuStartGame.Click
        startGame()
    End Sub

    Private Sub MenuUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuUpdate.Click
        setVersionSelected("0")
        updateGameVersion()
    End Sub

    Private Sub MenuUpdateMirror_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuUpdateMirror.Click
        setVersionSelected("1")
        updateGameVersion()
    End Sub

    Private Sub MenuFroggFr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuFroggFr.Click
        Process.Start(webUrl)
    End Sub

    Private Sub MenuForum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuForum.Click
        Process.Start(forumUrl)
    End Sub

    Private Sub MenuFacebook_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuFacebook.Click
        Process.Start(facebookUrl)
    End Sub

#End Region


#Region "Form Actions"

    'Form Loading
    Private Sub FroggPTConfigurator_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'hide toolbar with code to prevent unknow crash
        ToolBarProgress1.Visible = False
        ToolBarProgress2.Visible = False
        'init news
        setNews()
        'random bg
        setRdmBg()
        'init values
        initValues()
        'get options from config file
        getValues()
        'set default checked menu
        setCheckedSrv()
        'send values to form
        setFormValues()
        'send values to custom form
        setCustomFormValues()
        'set as loaded
        isLoaded = True
        'check game version
        cThread = New System.Threading.Thread(AddressOf checkGameVersion)
        cThread.Start()
    End Sub

    'Screen type change load new values
    Private Sub OptScreenT_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptScreenT.Click
        If isLoaded Then
            setScreenSList(st(OptScreenT.SelectedItem), st(OptScreenT.SelectedItem)(0))
            OptScreenS.SelectedIndex = 0
            checkResolution()
        End If
    End Sub

    'check input check values
    Private Sub OptFullScreen_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptFullScreen.Click
        checkResolution()
    End Sub

#End Region


#Region "Main Actions"

    Private Sub setNews()
        PanelLchNews.Url = New Uri(newsServer)
    End Sub

    Private Sub exitMain()
        System.Environment.Exit(-1)
        System.Windows.Forms.Application.Exit()
        cThread.abort()
        Close()
    End Sub

    Private Sub startGame()
        Process.Start(gameFile)
        exitMain()
    End Sub

    Private Sub setRdmBg()
        Dim random As New Random(), imgNb = random.Next(1, 20)
        While oldRdm = imgNb
            imgNb = random.Next(1, 20)
        End While
        Select Case imgNb
            Case 1
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond1
            Case 2
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond2
            Case 3
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond3
            Case 4
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond4
            Case 5
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond5
            Case 6
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond6
            Case 7
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond7
            Case 8
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond8
            Case 9
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond9
            Case 10
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond10
            Case 11
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond11
            Case 12
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond12
            Case 13
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond13
            Case 14
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond14
            Case 15
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond15
            Case 16
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond16
            Case 17
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond17
            Case 18
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond18
            Case 19
                PanelImgBg.BackgroundImage = My.Resources.Resources.fond19
        End Select
        oldRdm = imgNb
    End Sub

#End Region


#Region "Configurator Functions"

    Private Shared removeditems As New ArrayList
    Private Shared oldRdm = 0

    'change list SreenS values
    Private Function setScreenSList(ByVal arrStr As String(), ByVal resolution As String)
        Dim isFound = False
        For Each str As String In arrStr
            If str = resolution Then
                'clean Screen items
                OptScreenS.Items.Clear()
                'add Screen items
                For Each res In arrStr
                    OptScreenS.Items.Add(res)
                Next
                isFound = True
            End If
        Next
        setScreenSList = isFound
    End Function

    'check valid values for resolution selected
    Private Sub checkResolution()
        If OptFullScreen.Checked = True Then
            'restore removed resolutions
            For Each it In removeditems
                OptScreenS.Items.Add(it)
            Next
            'clean removeditem list
            removeditems.Clear()
        Else
            'if full windowed mode only < 1280 resolution are available
            If OptScreenS.SelectedIndex >= resMax Then
                OptScreenS.SelectedIndex = 0
            End If
            'clean removeditem list
            removeditems.Clear()
            'remove incompatible resolution
            While OptScreenS.Items.Count > resMax
                removeditems.Add(OptScreenS.Items(resMax).ToString)
                OptScreenS.Items.Remove(OptScreenS.Items(resMax))
            End While
        End If
    End Sub

    Private Sub showOption()
        MenuCustomize.Text = "[  Customize ]"
        MenuOptions.Text = "[ Launcher ]"
        PanelLauncher.Visible = False
        PanelCustom.Visible = False
        PanelOption.Visible = True
    End Sub

    Private Sub showLauncher()
        MenuOptions.Text = "[  Options  ]"
        MenuCustomize.Text = "[  Customize ]"
        PanelOption.Visible = False
        PanelCustom.Visible = False
        PanelLauncher.Visible = True
    End Sub

    Private Sub showCustom()
        MenuOptions.Text = "[  Options  ]"
        MenuCustomize.Text = "[ Launcher ]"
        PanelOption.Visible = False
        PanelLauncher.Visible = False
        PanelCustom.Visible = True
    End Sub

    Private Sub moveSoundFile(ByVal Move As String, ByVal remove As String)
        'remane soud files & folders
        For Each fold In sndFold
            If My.Computer.FileSystem.DirectoryExists(fold & Move) Then My.Computer.FileSystem.RenameDirectory(fold & Move, fold & remove)
        Next
        For Each file In sndFile
            If My.Computer.FileSystem.FileExists(file & Move) Then My.Computer.FileSystem.RenameFile(file & Move, GetFilenameFromPath(file & remove))
        Next
    End Sub

#End Region


#Region "Version Functions"

    Private clientVersionInt As Integer
    Private versionList As Array
    Private Shared startFrom As Integer = 0

    'check if game version if uptodate
    Private Sub checkGameVersion()

        Dim w As Integer = 0, lastVersion = "", webReq As Net.WebRequest, webRes As Net.WebResponse, inStream As StreamReader, versionUpToDate = False

        Try
            'get request file on version web server
            webReq = WebRequest.Create(versionServer)
            webRes = webReq.GetResponse()
            inStream = New StreamReader(webRes.GetResponseStream())
            versionList = inStream.ReadToEnd().Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries)

            'check if client version exist
            If clientVersion = "" Then
                clientVersion = firstVersion
                displayMsg("Version not found", 1)
            End If

            'get last server version
            For Each elem As String In versionList
                lastVersion = elem
                If lastVersion = clientVersion Then startFrom = w
                w = w + 1
            Next

            'convert strings to int
            clientVersionInt = Val(clientVersion)
            'test version
            If clientVersionInt = Val(lastVersion) Then
                versionUpToDate = True
                enablePlay()
                displayMsg("Your Version " & clientVersionInt & " is Up to date ! ", 1)
            Else
                displayMsg("Your Version " & clientVersionInt & " is OutDated ", 1)
            End If

        Catch ex As Exception
            displayMsg("An error occured while contacting Version server : " & vbCrLf & ex.Message, -1)
        End Try

        'version not up to date
        If Not versionUpToDate Then
            enableUpdate()
            If displayMsg("Your Version " & clientVersionInt & " is OutDated " & vbCrLf & "Last version is " & lastVersion & vbCrLf & vbCrLf & "Do you wish to update your FroggPT version to " & lastVersion, 3) Then updateGameVersion()
        End If

    End Sub

    'Add: cannot leave while updating
    Private Sub updateGameVersion()
        Dim nbVersion As Integer, notInit As Boolean, version As String
        'get version length
        nbVersion = (versionList.Length - 1)
        'get last server version
        For j As Integer = startFrom To nbVersion
            version = versionList(j)
            'For Each version As String In versionList
            If Not Trim(version) = "" Then
                If Val(version) > clientVersionInt Then
                    If Not notInit Then
                        initProgressBar(ToolBarProgress1, j, nbVersion + 1)
                        notInit = True
                    End If
                    displayMsg("Updating version to " & version, 1)
                    If Not downloadGameVersion(version) Then
                        ToolBarProgress1.Visible = False
                        Exit Sub
                    End If
                    ToolBarProgress1.Value = j + 1
                    System.Threading.Thread.Sleep(1000)
                End If
            End If
        Next
        'hide progress bar
        ToolBarProgress1.Visible = False
        'check if version has been updated
        checkGameVersion()
    End Sub

    'download file for a game version
    Private Function downloadGameVersion(ByVal version As String)
        Dim inStream As StreamReader, webReq As Net.WebRequest, webRes As Net.WebResponse, versionFileList As Array, nbFile As Integer, currFile As String, currServer As String, doResume As Boolean, resumeFile = ""
        Try
            'remove resume version as all has been downloaded
            If My.Computer.FileSystem.FileExists(resumeVersion) Then
                'get current version files
                doResume = True
                'get filename to start resume
                resumeFile = getLineAtFromFile(resumeVersion, -1)
                'if filename doesnt exist then cancel resume mode
                If Trim(resumeFile) = "" Then
                    doResume = False
                    'create the resume file as blank
                    File.Create(resumeVersion).Dispose()
                End If
                displayMsg("Resuming download", 1)
            Else
                'create resumeVersion files
                doResume = False
                'create the resume file as blank
                File.Create(resumeVersion).Dispose()
            End If

            'get selected server infos
            If ServerVersion = "0" Then currServer = versionServer Else currServer = versionMirrorServer
            'get file list
            webReq = WebRequest.Create(currServer & versionParam & version)
            webRes = webReq.GetResponse()
            inStream = New StreamReader(webRes.GetResponseStream())
            versionFileList = inStream.ReadToEnd().Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries)
            'nb files in patch folder
            nbFile = versionFileList.Length - 1
            'init progress bar values
            initProgressBar(ToolBarProgress2, 0, nbFile + 1)
            'init download object
            Dim DL = New WebClient
            'for each file in version list, download file
            For i As Integer = 0 To nbFile
                'downloading file
                currFile = versionFileList(i)
                If Not Trim(currFile) = "" Then
                    'displayMsg("downloading " & file, 1)
                    'test if stop to resume...or download file
                    If doResume Then
                        If resumeFile = currFile Then doResume = False
                    Else
                        'DOWNLOAD FILE !
                        Dim targetFile = Replace(currFile, version & "/", "")
                        'MsgBox("DOWNLOADING " & currServer & currFile & " TO " & targetFile)
                        If My.Computer.FileSystem.FileExists(targetFile) Then My.Computer.FileSystem.DeleteFile(targetFile)
                        'My.Computer.Network.DownloadFile(currServer & currFile, targetFile)
                        DL.DownloadFile(currServer & currFile, targetFile)
                        'Add downloaded File To resumeVersion 
                        My.Computer.FileSystem.WriteAllText(resumeVersion, vbCrLf & currFile, True)
                    End If
                End If
                ToolBarProgress2.Value = i + 1
            Next
            'update client version file
            clientVersion = version
            clientVersionInt = Val(version)
            setValues()
            'hide progress bar
            ToolBarProgress2.Visible = False
            'confirm message
            displayMsg("Version " & version & " installed", 1)
            'remove resume version as all has been downloaded
            If My.Computer.FileSystem.FileExists(resumeVersion) Then My.Computer.FileSystem.DeleteFile(resumeVersion)
            Return True
        Catch ex As Exception
            'hide button update
            displayMsg("An error occured while updating game version : " & vbCrLf & ex.Message, -1)
            Return False
        End Try
    End Function

    'enable play button
    Private Sub enablePlay()
        MenuStartGame.Enabled = True
        MenuUpdate.Enabled = False
        MenuUpdateMirror.Enabled = False
        PanelLchPlay.Image = My.Resources.Resources.play
        PanelLchPlay.Enabled = True
    End Sub

    'enable update button
    Public Sub enableUpdate()
        MenuStartGame.Enabled = False
        MenuUpdate.Enabled = True
        MenuUpdateMirror.Enabled = True
        PanelLchPlay.Enabled = False
    End Sub

    'set version server to check
    Private Sub setVersionSelected(ByVal srv As String)
        'set version server to check
        ServerVersion = srv
        'set checked status
        setCheckedSrv()
        'save to file
        setValues()
    End Sub

    'add check status to menu
    Private Sub setCheckedSrv()
        If ServerVersion = "0" Then
            MenuUpdateMirror.Checked = False
            MenuUpdate.Checked = True
        Else
            MenuUpdateMirror.Checked = True
            MenuUpdate.Checked = False
        End If
    End Sub

#End Region


#Region "Custom Functions"

    Private Shared currentFolder = Directory.GetCurrentDirectory()
    Private Shared tempFileName = "temp.zip"

    Private Sub setCustomFormValues()
        If CustomCharacterSelect = 0 Then PanelCstmCO.Checked = True Else PanelCstmCC.Checked = True
        If CustomInterface = 0 Then PanelCstmIO.Checked = True Else PanelCstmIC.Checked = True
        If CustomOpening = 0 Then PanelCstmOO.Checked = True Else PanelCstmOC.Checked = True
        If CustomSound = 0 Then PanelCstmSO.Checked = True Else PanelCstmSC.Checked = True
    End Sub

    Private Sub getCustomValues()
        If PanelCstmCO.Checked = True Then CustomCharacterSelect = "0" Else CustomCharacterSelect = "1"
        If PanelCstmIO.Checked = True Then CustomInterface = "0" Else CustomInterface = "1"
        If PanelCstmOO.Checked = True Then CustomOpening = "0" Else CustomOpening = "1"
        If PanelCstmSO.Checked = True Then CustomSound = "0" Else CustomSound = "1"
    End Sub

    Private Sub setCustomValues()
        Dim f = currentFolder & "\" & tempFileName, c = currentFolder

        'prepare
        displayMsg("Updating...", 1)
        initProgressBar(ToolBarProgress1, 0, 4)

        'restore sound file
        moveSoundFile("z", "")

        'copying file for each case
        If CustomCharacterSelect = 1 Then extractResourceZip(f, c, My.Resources.Resources._modified_CharacterSelect) Else extractResourceZip(f, c, My.Resources.Resources._original_CharacterSelect)
        displayMsg("Character Select Files Updated", 1)
        ToolBarProgress1.Value = 1

        If CustomInterface = 1 Then extractResourceZip(f, c, My.Resources.Resources._modified_Interface) Else extractResourceZip(f, c, My.Resources.Resources._original_Interface)
        displayMsg("Interface Files Updated", 1)
        ToolBarProgress1.Value = 2

        If CustomOpening = 1 Then extractResourceZip(f, c, My.Resources.Resources._modified_Opening) Else extractResourceZip(f, c, My.Resources.Resources._original_Opening)
        ToolBarProgress1.Value = 3
        displayMsg("Opening Files Updated", 1)

        If CustomSound = 1 Then extractResourceZip(f, c, My.Resources.Resources._modified_Sound) Else extractResourceZip(f, c, My.Resources.Resources._original_Sound)
        ToolBarProgress1.Value = 4
        displayMsg("Sound Files Updated", 1)

        ToolBarProgress1.Visible = False

        setValues()
    End Sub

#End Region


#Region "Buttons"

    '======[ SAVE BUTTON ]======

    Private Sub PanelOptSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelOptSave.Click
        'get option from form
        getFormValues()
        'send values to config file
        setValues()
        'msg ok
        displayMsg("Settings saved ^_^", 0)
        displayMsg("Settings saved ^_^", 1)
        'Show luancher window
        showLauncher()
    End Sub

    Private Sub PanelOptSave_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelOptSave.MouseHover
        PanelOptSave.Image = My.Resources.Resources.save_o
    End Sub

    Private Sub PanelOptSave_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelOptSave.MouseLeave
        PanelOptSave.Image = My.Resources.Resources.save
    End Sub

    Private Sub PanelOptSave_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelOptSave.MouseDown
        PanelOptSave.Image = My.Resources.Resources.save_a
    End Sub

    '=====[ OPTION BUTTON ]=====

    Private Sub PanelLchOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchOptions.Click
        showOption()
    End Sub

    Private Sub PanelLchOptions_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchOptions.MouseHover
        PanelLchOptions.Image = My.Resources.Resources.options_o
    End Sub

    Private Sub PanelLchOptions_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchOptions.MouseLeave
        PanelLchOptions.Image = My.Resources.Resources.options
    End Sub

    Private Sub PanelLchOptions_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchOptions.MouseDown
        PanelLchOptions.Image = My.Resources.Resources.options_a
    End Sub

    '======[ PLAY BUTTON ]======

    Private Sub PanelLchPlay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchPlay.Click
        startGame()
    End Sub

    Private Sub PanelLchPlay_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchPlay.MouseHover
        PanelLchPlay.Image = My.Resources.Resources.play_o
    End Sub

    Private Sub PanelLchPlay_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchPlay.MouseLeave
        PanelLchPlay.Image = My.Resources.Resources.play
    End Sub

    Private Sub PanelLchPlay_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelLchPlay.MouseDown
        PanelLchPlay.Image = My.Resources.Resources.play_a
    End Sub

    '======[ CLOSE BUTTON ]======

    Private Sub PanelBtnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelBtnClose.Click
        exitMain()
    End Sub

    Private Sub PanelBtnClose_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelBtnClose.MouseHover
        PanelBtnClose.Image = My.Resources.Resources.close_o
    End Sub

    Private Sub PanelBtnClose_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelBtnClose.MouseLeave
        PanelBtnClose.Image = My.Resources.Resources.close
    End Sub

    '======[ SAVE BUTTON ]======

    Private Sub PanelCstmSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelCstmSave.Click
        'get option from form
        getCustomValues()
        'send values to config file
        setCustomValues()
        'msg ok
        displayMsg("Custom options saved ^_^", 0)
        displayMsg("Custom options ^_^", 1)
        'Show luancher window
        showLauncher()
    End Sub

    Private Sub PanelCstmSave_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelCstmSave.MouseHover
        PanelOptSave.Image = My.Resources.Resources.save_o
    End Sub

    Private Sub PanelCstmSave_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelCstmSave.MouseLeave
        PanelOptSave.Image = My.Resources.Resources.save
    End Sub

    Private Sub PanelCstmSave_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PanelCstmSave.MouseDown
        PanelOptSave.Image = My.Resources.Resources.save_a
    End Sub

#End Region


#Region "Functions"

    'get key from a value in dictionary
    Private Function getKeyFromDictionary(ByVal Dic As Dictionary(Of String, String), ByVal strItem As String) As String
        'Init Loop over entries.
        Dim kv As KeyValuePair(Of String, String)
        'Loop over entries.
        For Each kv In Dic
            'if key is resolution then mean it is selected
            If kv.Value = strItem Then
                getKeyFromDictionary = kv.Key
                Exit Function
            End If
        Next
        getKeyFromDictionary = ""
    End Function

    'Create a file
    Private Sub writeFile(ByVal fileName As String, ByVal lines As ArrayList)
        'create stream file
        Dim streamFile As New System.IO.StreamWriter(fileName, False)
        'add each line
        For Each line In lines
            streamFile.WriteLine(line)
        Next
        'end file stream
        streamFile.Close()
    End Sub

    'get line at nb from file
    Private Function getLineAtFromFile(ByVal fileName As String, ByVal nb As Integer) '-1 for last line
        Dim lines() As String = IO.File.ReadAllLines(fileName)
        If lines.Length > 0 Then
            If nb = -1 Then nb = lines.Length - 1
            getLineAtFromFile = lines(nb)
        Else
            getLineAtFromFile = ""
        End If
    End Function

    'get filename from a path
    Private Function GetFilenameFromPath(ByVal path As String) As String
        GetFilenameFromPath = Split(path, "\")(UBound(Split(path, "\")))
    End Function

    'Display msg
    Public Function displayMsg(ByVal msg As String, ByVal lvl As Integer)
        'lvl -1 = error
        'lvl  0 = ok
        'lvl  1 = info
        'lvl  2 = waring

        Dim msgTitle = "FroggPT launcher message", btn As Integer

        'define msg box title depending in lvl value
        Select Case lvl
            Case -1
                msgTitle = "Error occured"
                btn = Nothing
                ToolBarProgress1.Visible = False
                ToolBarProgress2.Visible = False
                ToolBarStatus.Text = msgTitle
            Case 1
                msgTitle = ""
                btn = Nothing
            Case 2
                msgTitle = "Warning occured"
                btn = Nothing
            Case 3
                msgTitle = "Make a choice"
                btn = MessageBoxButtons.YesNo
        End Select

        'if no title is set then no msg box pop
        If Not msgTitle = "" Then
            If btn = MessageBoxButtons.YesNo Then
                Dim answer As DialogResult
                answer = MsgBox(msg, btn, msgTitle)
                If answer = vbYes Then displayMsg = True Else displayMsg = False
            Else
                MsgBox(msg, btn, msgTitle)
                displayMsg = True
            End If
        Else
            ToolBarStatus.Text = msg
            displayMsg = True
        End If

    End Function

    'init a progress bar
    Private Sub initProgressBar(ByVal progress As ToolStripProgressBar, ByVal s As Integer, ByVal e As Integer)
        progress.Visible = True
        progress.Minimum = s
        progress.Maximum = e
    End Sub


    Private Sub writeByteFile(ByVal name As String, ByVal resource As Byte())
        Dim FileStream As New System.IO.FileStream(IO.Path.Combine(Application.StartupPath, name), System.IO.FileMode.OpenOrCreate)
        Dim BinaryWriter As New System.IO.BinaryWriter(FileStream)
        BinaryWriter.Write(resource)
        BinaryWriter.Close()
        FileStream.Close()
    End Sub

    Private Sub extractResourceZip(ByVal zipFileName As String, ByVal outputFolder As String, ByVal byteFile As Byte())
        'create temp zip file
        writeByteFile(zipFileName, byteFile)
        'extract zip file
        extractZip(zipFileName, outputFolder)
        'Remove temp file
        If My.Computer.FileSystem.FileExists(zipFileName) Then My.Computer.FileSystem.DeleteFile(zipFileName)
    End Sub

    Private Sub extractZip(ByVal zipFileName As String, ByVal outputFolder As String)
        Dim shObj As Object = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"))
        'create a temp folder
        My.Computer.FileSystem.CreateDirectory(outputFolder & "\temp")
        'Declare the folder where the items will be extracted.
        Dim output As Object = shObj.NameSpace((outputFolder & "\temp"))
        'Declare the input zip file.
        Dim input As Object = shObj.NameSpace((zipFileName))
        'Extract the items from the zip file.
        For Each item In input.Items
            'copy extracted files to temp folder
            output.CopyHere(item, 4)
            'copy files
            My.Computer.FileSystem.MoveDirectory(outputFolder & "\temp", outputFolder, True)
        Next
    End Sub

#End Region


#Region " Move Form "

    Private MoveForm As Boolean
    Private MoveForm_MousePosition As Point

    Private Sub MoveForm_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelContainALL.MouseDown
        If e.Button = MouseButtons.Left Then
            MoveForm = True
            Me.Cursor = Cursors.NoMove2D
            MoveForm_MousePosition = e.Location
        End If
    End Sub

    Private Sub MoveForm_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelContainALL.MouseMove
        If MoveForm Then Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
    End Sub

    Private Sub MoveForm_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelContainALL.MouseUp
        If e.Button = MouseButtons.Left Then
            MoveForm = False
            Me.Cursor = Cursors.Default
        End If
    End Sub

    ''THIS WORKS TOO ===>
    'Private IsFormBeingDragged As Boolean = False
    'Private MouseDownX As Integer
    'Private MouseDownY As Integer

    'Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Panel1.MouseDown
    '    If e.Button = MouseButtons.Left Then
    '        IsFormBeingDragged = True
    '        MouseDownX = e.X
    '        MouseDownY = e.Y
    '    End If
    'End Sub

    'Private Sub Form1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Panel1.MouseUp
    '    If e.Button = MouseButtons.Left Then IsFormBeingDragged = False
    'End Sub

    'Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Panel1.MouseMove
    '    If IsFormBeingDragged Then
    'Dim temp As Point = New Point()
    '        temp.X = Me.Location.X + (e.X - MouseDownX)
    '        temp.Y = Me.Location.Y + (e.Y - MouseDownY)
    '        Me.Location = temp
    '        temp = Nothing
    '    End If
    'End Sub

#End Region

    Private Sub PanelLchOptions_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PanelLchOptions.MouseDown

    End Sub
    Private Sub PanelLchPlay_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PanelLchPlay.MouseDown

    End Sub
    Private Sub PanelOptSave_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PanelOptSave.MouseDown

    End Sub
    Private Sub PanelCstmSave_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PanelCstmSave.MouseDown

    End Sub
End Class