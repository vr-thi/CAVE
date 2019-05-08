Imports System.ComponentModel
Imports System.IO
Imports System.Net

Public Class MainForm
    Dim computers As New List(Of Computer)
    Dim act_computer As Computer
    Dim currNodeConfigPath As String
    Dim projectDirPath As String
    Dim projectName As String
    Const streamingAssetsDir As String = "_Data\StreamingAssets\"
    Const nodeConfigFileName As String = "node-config.xml"

    Dim progressCount As Integer
    Private Results As String
    Private Delegate Sub delUpdate()
    Private Finished As New delUpdate(AddressOf UpdateText)
    Private Delegate Sub buttonsUpdate()
    Private FinishedButtons As New buttonsUpdate(AddressOf EnableButtons)
    Dim button_click As Integer = 0
    Dim pf As New ProjectForm
    Dim P As Process
    'Dim configfilepath As String = ".\config.xml"
    Dim scripts As String = ".\user_scripts\"
    Dim xmlreadconfig As XMLReader = New XMLReader
    Dim pathToSlave As String = ""
    Dim newPath As String = ""
    Dim CMDThread As Threading.Thread
    Dim vrpnServerPath As String = Application.StartupPath() & "\..\VRPN_Server\"
    Dim vrpnServerFile As String = "vrpn_server.exe"
    Dim AutostartOnMaster As Boolean = True
    Dim autoSaveConfigNode As Boolean = True
    Dim asNewCnfigNodeSaved As Boolean = False
    Dim cmdThreadWithStartTaste As Boolean = False

    Private Sub clearAllFields()
        txt_ipAddress.Text = ""
        txt_port.Text = ""
        txt_computerName.Text = ""
        chk_master.Checked = False
        list_rela.SelectedIndex = 0
        txt_relat_x.Text = ""
        txt_relat_y.Text = ""
        txt_relat_z.Text = ""
        list_cam.SelectedIndex = 0
        txt_cam_x.Text = ""
        txt_cam_y.Text = ""
        txt_cam_z.Text = ""
        list_splane.SelectedIndex = 0
        txt_splane_x.Text = ""
        txt_splane_y.Text = ""
        txt_splane_z.Text = ""
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Me.grp_computerInfo.Text = "Configuration of " & Me.ListBox1.SelectedItem.ToString
        'clearAllFields()
        act_computer = computers.ElementAt(Me.ListBox1.SelectedIndex)
        txt_ipAddress.Text = act_computer.ip
        txt_computerName.Text = act_computer.name
        If TypeOf act_computer Is Master Then
            chk_master.Checked = True
            chk_master.Enabled = False
            grp_relation.Enabled = True
            grp_cam.Enabled = False
            txt_port.Enabled = True
            txt_port.Text = DirectCast(act_computer, Master).port
            list_rela.SelectedIndex = 0
        Else
            txt_port.Text = ""
            txt_port.Enabled = False
            chk_master.Checked = False
            chk_master.Enabled = False
            grp_relation.Enabled = False
            grp_cam.Enabled = True
            list_cam.SelectedIndex = 0
            If DirectCast(act_computer, Slave).camera.eye.Equals("left") Then
                combo_eye.SelectedIndex = 0
            Else
                combo_eye.SelectedIndex = 1
            End If
        End If
        grp_computerInfo.Enabled = True
        grp_splane.Enabled = True
        list_splane.SelectedIndex = 0
    End Sub

    Private Sub ÖffnenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ÖffnenToolStripMenuItem.Click
        If FolderBrowserDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Me.DeleteConfigNode()
            Me.asNewCnfigNodeSaved = False

            projectDirPath = FolderBrowserDialog1.SelectedPath
            projectName = projectDirPath.Substring(projectDirPath.LastIndexOf("\") + 1)
            projectDirPath += "\"

            If IO.Directory.Exists(projectDirPath + projectName + streamingAssetsDir) Then
                For Each item As String In IO.Directory.GetFiles(projectDirPath + projectName + streamingAssetsDir, "*.xml")
                    If IO.Path.GetFileName(item) = Me.nodeConfigFileName Then
                        Me.asNewCnfigNodeSaved = False
                        MsgBox("NEW IN THIS VERSION: " + vbNewLine + vbNewLine +
                           "The file 'node-config.xml' works only as workcopy of node configuration." + vbNewLine +
                           "Project node configuration should be saved as e.g.:" + vbNewLine +
                           "'node-config-cave.xml' or 'node-config-local.xml'." + vbNewLine + vbNewLine +
                           "Please select new name for your 'node-config.xml' file!",
                            MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
                        Me.SaveConfigNodeAs(item)
                    Else
                        Me.asNewCnfigNodeSaved = True
                    End If
                Next
                Me.LoadConfigFiles()
            Else
                MsgBox("Selected project folder seems to be wrong - streaming assets directory does not exist!" + vbNewLine + "Please, choose proper project directory.", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly)
            End If
        End If
    End Sub

    Private Sub cbConfigNodes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbConfigNodes.SelectedIndexChanged
        currNodeConfigPath = projectDirPath + projectName + streamingAssetsDir + Me.cbConfigNodes.Text '+ ".xml"

        If File.Exists(currNodeConfigPath) Then
            ListBox1.Items.Clear()

            Dim xmlReader As New XMLReader
            computers = xmlReader.GetAllComputers(currNodeConfigPath)

            For Each computer In computers
                ListBox1.Items.Add(computer.ToString)
            Next
        Else
            MsgBox("File: " + vbNewLine + "'" + currNodeConfigPath + "'" + vbNewLine + "does not exist." + vbNewLine + "Please choose correct directory of project!")
        End If
    End Sub

    Private Sub list_rela_SelectedIndexChanged(sender As Object, e As EventArgs) Handles list_rela.SelectedIndexChanged
        Dim relation As Origin = DirectCast(act_computer, Master).origin

        If list_rela.SelectedItem.Equals("Position") Then
            txt_relat_x.Text = relation.position.x
            txt_relat_y.Text = relation.position.y
            txt_relat_z.Text = relation.position.z
        ElseIf list_rela.SelectedItem.Equals("Rotation") Then
            txt_relat_x.Text = relation.rotation.x
            txt_relat_y.Text = relation.rotation.y
            txt_relat_z.Text = relation.rotation.z
        End If
    End Sub

    Private Sub list_splane_SelectedIndexChanged(sender As Object, e As EventArgs) Handles list_splane.SelectedIndexChanged
        Dim screenplane As ScreenPlane = act_computer.screenplane

        If list_splane.SelectedItem.Equals("Position") Then
            txt_splane_x.Enabled = True
            txt_splane_y.Enabled = True
            txt_splane_z.Enabled = True
            txt_scale.Enabled = False
            txt_splane_x.Text = screenplane.position.x
            txt_splane_y.Text = screenplane.position.y
            txt_splane_z.Text = screenplane.position.z
        ElseIf list_splane.SelectedItem.Equals("Rotation") Then
            txt_splane_x.Enabled = True
            txt_splane_y.Enabled = True
            txt_splane_z.Enabled = True
            txt_scale.Enabled = False
            txt_splane_x.Text = screenplane.rotation.x
            txt_splane_y.Text = screenplane.rotation.y
            txt_splane_z.Text = screenplane.rotation.z
        ElseIf list_splane.SelectedItem.Equals("Scale") Then
            txt_splane_x.Enabled = False
            txt_splane_y.Enabled = False
            txt_splane_z.Enabled = False
            txt_scale.Enabled = True
            txt_scale.Text = screenplane.scale.x
        End If
    End Sub

    Private Sub list_cam_SelectedIndexChanged(sender As Object, e As EventArgs) Handles list_cam.SelectedIndexChanged
        Dim camera As Camera = DirectCast(act_computer, Slave).camera

        If list_cam.SelectedItem.Equals("Rotation") Then
            txt_cam_x.Text = camera.rotation.x
            txt_cam_y.Text = camera.rotation.y
            txt_cam_z.Text = camera.rotation.z
        End If
    End Sub

    Private Sub BeendenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeendenToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub btn_configSave_Click(sender As Object, e As EventArgs) Handles btn_configSave.Click
        Dim xmlWriter As New XMLWriter

        xmlWriter.SetAllComputers(currNodeConfigPath, computers)
        MsgBox("Configuration saved", MsgBoxStyle.OkOnly, "Configuration saved")
    End Sub

    Private Sub txt_ipAddress_TextChanged(sender As Object, e As EventArgs) Handles txt_ipAddress.TextChanged
        act_computer.ip = txt_ipAddress.Text

        Dim position_lb1 As Integer = ListBox1.SelectedIndex

        ListBox1.Items.Clear()
        For Each computer In computers
            ListBox1.Items.Add(computer.ToString)
        Next
        ListBox1.SelectedIndex = position_lb1
    End Sub

    Private Sub txt_port_TextChanged(sender As Object, e As EventArgs) Handles txt_port.TextChanged
        If TypeOf act_computer Is Master Then
            DirectCast(act_computer, Master).port = txt_port.Text
        End If
    End Sub

    Private Sub txt_computerName_TextChanged(sender As Object, e As EventArgs) Handles txt_computerName.TextChanged
        act_computer.name = txt_computerName.Text
    End Sub

    Private Sub txt_relat_x_TextChanged(sender As Object, e As EventArgs) Handles txt_relat_x.TextChanged
        If list_rela.SelectedItem.Equals("Position") Then
            DirectCast(act_computer, Master).origin.position.x = txt_relat_x.Text
        ElseIf list_rela.SelectedItem.Equals("Rotation") Then
            DirectCast(act_computer, Master).origin.rotation.x = txt_relat_x.Text
        End If
    End Sub

    Private Sub txt_cam_x_TextChanged(sender As Object, e As EventArgs) Handles txt_cam_x.TextChanged
        DirectCast(act_computer, Slave).camera.rotation.x = txt_cam_x.Text
    End Sub

    Private Sub txt_splane_x_TextChanged(sender As Object, e As EventArgs) Handles txt_splane_x.TextChanged
        If list_splane.SelectedItem.Equals("Position") Then
            act_computer.screenplane.position.x = txt_splane_x.Text
        ElseIf list_splane.SelectedItem.Equals("Rotation") Then
            act_computer.screenplane.rotation.x = txt_splane_x.Text
        End If
    End Sub

    Private Sub txt_relat_y_TextChanged(sender As Object, e As EventArgs) Handles txt_relat_y.TextChanged
        If list_rela.SelectedItem.Equals("Position") Then
            DirectCast(act_computer, Master).origin.position.y = txt_relat_y.Text
        ElseIf list_rela.SelectedItem.Equals("Rotation") Then
            DirectCast(act_computer, Master).origin.rotation.y = txt_relat_y.Text
        End If
    End Sub

    Private Sub txt_relat_z_TextChanged(sender As Object, e As EventArgs) Handles txt_relat_z.TextChanged
        If list_rela.SelectedItem.Equals("Position") Then
            DirectCast(act_computer, Master).origin.position.z = txt_relat_z.Text
        ElseIf list_rela.SelectedItem.Equals("Rotation") Then
            DirectCast(act_computer, Master).origin.rotation.z = txt_relat_z.Text
        End If
    End Sub

    Private Sub txt_cam_y_TextChanged(sender As Object, e As EventArgs) Handles txt_cam_y.TextChanged
        DirectCast(act_computer, Slave).camera.rotation.y = txt_cam_y.Text
    End Sub

    Private Sub txt_cam_z_TextChanged(sender As Object, e As EventArgs) Handles txt_cam_z.TextChanged
        DirectCast(act_computer, Slave).camera.rotation.z = txt_cam_z.Text
    End Sub

    Private Sub txt_splane_y_TextChanged(sender As Object, e As EventArgs) Handles txt_splane_y.TextChanged
        If list_splane.SelectedItem.Equals("Position") Then
            act_computer.screenplane.position.y = txt_splane_y.Text
        ElseIf list_splane.SelectedItem.Equals("Rotation") Then
            act_computer.screenplane.rotation.y = txt_splane_y.Text
        End If
    End Sub

    Private Sub txt_splane_z_TextChanged(sender As Object, e As EventArgs) Handles txt_splane_z.TextChanged
        If list_splane.SelectedItem.Equals("Position") Then
            act_computer.screenplane.position.z = txt_splane_z.Text
        ElseIf list_splane.SelectedItem.Equals("Rotation") Then
            act_computer.screenplane.rotation.z = txt_splane_z.Text
        End If
    End Sub

    Private Sub combo_eye_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combo_eye.SelectedIndexChanged
        If combo_eye.SelectedIndex.Equals(0) Then
            DirectCast(act_computer, Slave).camera.eye = "left"
        Else
            DirectCast(act_computer, Slave).camera.eye = "right"
        End If
    End Sub

    Private Sub grp_computerInfo_MouseHover(sender As Object, e As EventArgs) Handles grp_computerInfo.MouseHover
        txt_info.Text = "Configure settings of Computer"
    End Sub

    Private Sub grp_computerList_MouseHover(sender As Object, e As EventArgs) Handles grp_computerList.MouseHover
        txt_info.Text = "List of Computers to configure"
    End Sub

    Private Sub opencmd_start()
        Dim args As String = ""
        Dim Pr As New Process
        Dim StartOnMaster As String = IIf(Me.AutostartOnMaster, "AutoStartOnMaster", "HandStartOnMaster")

        If VRPNstate() = False Then
            Try
                Process.Start(vrpnServerPath & vrpnServerFile, "-f " & vrpnServerPath & "vrpn.cfg")
            Catch ex As Exception
                MsgBox(ex)
            Finally
                Me.VRPNstate()
            End Try
        End If
        P = Pr
        Pr.Close()
        P.StartInfo.CreateNoWindow = True
        P.StartInfo.UseShellExecute = False
        P.StartInfo.RedirectStandardInput = True
        P.StartInfo.RedirectStandardOutput = True
        P.StartInfo.RedirectStandardError = True
        P.StartInfo.FileName = ".\StartProject.bat"

        ' MASTER_StartUnity bekommt als Argumente folgenden Pfad:
        ' - Unity-Projekt-Exe-Datei
        ' um die Datei zu starten
        If File.Exists(projectDirPath + projectName + ".exe") Then
            P.StartInfo.Arguments = String.Format("""{0}{1}.exe"" ""{1}"" {2}", projectDirPath, projectName, StartOnMaster)
        End If
        P.Start()
    End Sub

    Private Sub opencmd_deployupdate(update As Boolean)
        Dim Pr As New Process
        P = Pr
        Pr.Close()
        P.StartInfo.CreateNoWindow = True
        P.StartInfo.UseShellExecute = False
        P.StartInfo.RedirectStandardInput = True
        P.StartInfo.RedirectStandardOutput = True
        P.StartInfo.RedirectStandardError = True
        P.StartInfo.FileName = ".\DeployProject.bat"
        If update Then
            P.StartInfo.Arguments = String.Format("""{0}"" ""{1}"" OnlyUpdate", projectDirPath, projectName)
        Else
            P.StartInfo.Arguments = String.Format("""{0}"" ""{1}""", projectDirPath, projectName)
        End If
        P.Start()
    End Sub

    Private Sub UpdateText()
        Try
            pf.txt_projectForm.AppendText(System.Environment.NewLine() & Results)
            'If Results.ToUpper.Contains("XCOPY ") Or Results.ToUpper.Contains("RMDIR ") Or Results.ToUpper.Contains("COPY ") Or Results.ToUpper.Contains("DEL ") Or Results.ToUpper.Contains("PSEXEC ") Then
            If Results.ToUpper.Contains("DONE ") Then
                pf.ProgressBar1.PerformStep()
                'progressCount = progressCount + 1
                'pf.Text = progressCount.ToString
            End If
            pf.txt_projectForm.ScrollToCaret()
        Catch ex As Exception
            MsgBox("Error. Process cancelled.", MsgBoxStyle.Critical, "Error")
            P.Close()
            Invoke(FinishedButtons)
            CMDThread.Abort()
        End Try
    End Sub

    Private Sub CMDConfig()
        While P.StandardOutput.EndOfStream = False
            Results = P.StandardOutput.ReadLine()
            Invoke(Finished)
        End While
        P.Close()
        Invoke(FinishedButtons)
        CMDThread.Abort()
        If Me.cmdThreadWithStartTaste Then
            Me.DeleteConfigNode()
        End If
    End Sub

    Private Sub EnableButtons()
        btn_deployProject.Enabled = True
        btn_startProject.Enabled = True
        btn_updateProject.Enabled = True
        pf.btn_OK.Enabled = True
    End Sub

    Private Sub DisableButtons()
        btn_deployProject.Enabled = False
        btn_startProject.Enabled = False
        btn_updateProject.Enabled = False
        pf.btn_OK.Enabled = False
    End Sub

    Private Sub btn_startProject_Click(sender As Object, e As EventArgs) Handles btn_startProject.Click
        'If File.Exists(".\MASTER_StartUnity.bat") Then
        If File.Exists(".\StartProject.bat") Then
            Dim pfd As New ProjectForm

            If Application.OpenForms().OfType(Of ProjectForm).Any Then
                pf.Close()
            End If

            DisableButtons()
            pf = pfd
            pf.txt_projectForm.Text = ""
            pf.Text = "Start Project"
            pf.Visible = True
            progressCount = 0
            pf.ProgressBar1.Value = 0
            pf.ProgressBar1.Maximum = (ListBox1.Items.Count - 1)
            opencmd_start()
            Me.cmdThreadWithStartTaste = True
            Dim CMDThread2 As New Threading.Thread(AddressOf CMDConfig)
            CMDThread = CMDThread2
            'start cmd thread
            CMDThread.Start()
        Else
            MsgBox("StartProject.bat not found", MsgBoxStyle.Critical, "Not Found")
        End If
    End Sub

    Private Sub checkStartUnity()
        'Dim fileBAT As File
        'Dim path As String = ".\SLAVE_StartUnity.bat"
        'Dim content As String = "echo Startet Unityprojekt auf dem Slave-Rechner %computername% " + vbNewLine + "start " + pathToSlave + projectName + ".exe"
        'fileBAT.WriteAllText(path, content)
    End Sub

    Private Sub btn_deployProject_Click(sender As Object, e As EventArgs) Handles btn_deployProject.Click, btn_updateProject.Click
        Dim isUpdate As Boolean = sender Is Me.btn_updateProject
        'Dim allowDeploy As Boolean = Me.autoSaveConfigNode

        'If Not allowDeploy Then
        '    allowDeploy = MsgBox("The 'config-node.xml' will be automaticly replace with current config-node configuration: " +
        '                         vbNewLine + filename + " !" + vbNewLine + vbNewLine +
        '                         "Would you like to continue?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes
        'End If
        'If allowDeploy Then
        If Me.asNewCnfigNodeSaved Then
            Dim XMLWriter As New XMLWriter
            XMLWriter.SetAllComputers(projectDirPath + projectName + streamingAssetsDir + "node-config.xml", computers)
        End If
        If File.Exists(".\DeployProject.bat") Then
            Dim pfd As New ProjectForm

            If Application.OpenForms().OfType(Of ProjectForm).Any Then
                pf.Close()
            End If

            DisableButtons()
            'checkStartUnity()
            pf = pfd
            pf.txt_projectForm.Text = ""
            pf.Text = IIf(isUpdate, "Update", "Deploy") + " project"
            pf.Visible = True
            progressCount = 0
            pf.ProgressBar1.Value = 0
            pf.ProgressBar1.Maximum = (ListBox1.Items.Count - 1)
            opencmd_deployupdate(isUpdate)
            Me.cmdThreadWithStartTaste = False
            Dim CMDThread2 As New Threading.Thread(AddressOf CMDConfig)
            CMDThread = CMDThread2
            'start cmd thread
            CMDThread.Start()
        Else
            MsgBox("File: 'DeployProject.bat' not found!", MsgBoxStyle.Critical, "Not Found")
        End If
        ' End If
    End Sub

    Private Sub ÜberDasToolToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ÜberDasToolToolStripMenuItem.Click
        MsgBox("CAVEUnity Deploy & Config Tool V1.0.4" + vbNewLine + vbNewLine + "Visit us on GitHub: http://www.github.com/vr-thi/CAVE/", MsgBoxStyle.Information, "CAVEUnity")
    End Sub

    Private Sub opencmd_deploy(v As String)
        Throw New NotImplementedException()
    End Sub

    Private Sub txt_scale_TextChanged(sender As Object, e As EventArgs) Handles txt_scale.TextChanged
        If list_splane.SelectedItem.Equals("Scale") Then
            act_computer.screenplane.scale.x = txt_scale.Text
            act_computer.screenplane.scale.y = txt_scale.Text
            act_computer.screenplane.scale.z = txt_scale.Text
        End If
    End Sub

    Private Sub SlaveConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SlaveConfigToolStripMenuItem.Click
        'If File.Exists(configfilepath) Then
        '    Dim newPath As String = ""
        '    Dim xmlRead As New XMLReader
        '    pathToSlave = xmlRead.GetConfig(configfilepath, "pathToSlave")
        '    newPath = InputBox("Please enter the path to the folder where the .exe-file is located:", "Slave Path", pathToSlave)
        '    If newPath IsNot "" Then
        '        If Not newPath.Equals(pathToSlave) Then
        '            Dim xmlWrite As New XMLWriter
        '            xmlWrite.SetConfig(configfilepath, "pathToSlave", newPath)
        '            pathToSlave = newPath
        '        End If
        '    End If
        'End If
    End Sub

    Private Sub btn_addComputer_Click(sender As Object, e As EventArgs) Handles btn_addComputer.Click
        Dim computer_new As New Slave()
        Dim camera As New Camera
        Dim rotation As New Vektor
        Dim screenplane As New ScreenPlane
        Dim position As New Vektor
        Dim sp_rotation As New Vektor
        Dim scale As New Vektor
        Dim vektor_cam As New Vektor

        computer_new.camera = camera
        computer_new.camera.rotation = rotation
        computer_new.screenplane = screenplane
        computer_new.screenplane.position = position
        computer_new.screenplane.rotation = sp_rotation
        computer_new.screenplane.scale = scale

        computers.Add(computer_new)
        ListBox1.Items.Add(computer_new.ToString)
    End Sub

    Private Sub btm_removeComputer_Click(sender As Object, e As EventArgs) Handles btm_removeComputer.Click
        Dim computerToRemove As Computer = computers.ElementAt(Me.ListBox1.SelectedIndex)

        If TypeOf computerToRemove Is Master Then
            MsgBox("A computer from type Master cannot be removed", MsgBoxStyle.Critical, "Remove Computer")
        Else
            Dim result As Integer = MessageBox.Show("Do you want to remove the computer?", "Remove Computer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information)
            If result = DialogResult.Yes Then
                computers.Remove(computerToRemove)
                ListBox1.Items.Clear()
                For Each computer In computers
                    ListBox1.Items.Add(computer.ToString)
                Next
            End If
        End If
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MakeUserScriptsMenu()
        'If File.Exists(configfilepath) Then
        '    pathToSlave = xmlreadconfig.GetConfig(configfilepath, "pathToSlave")
        'Else
        '    MsgBox("Error: config.xml not found!", MsgBoxStyle.Exclamation)
        '    Me.Close()
        'End If
    End Sub

    Private Sub MakeUserScriptsMenu()
        Me.ScriptsToolStripMenuItem.Visible = False
        If IO.Directory.Exists(scripts) Then
            Dim allowExts() As String = {".bat", ".bin", ".cmd", ".com", ".cpl", ".exe", ".ins", ".inx", ".isu", ".job", ".jse", ".lnk", ".msc", ".ps1", ".vb", ".vbs", ".ws", ".wsf"}
            Dim img As Image
            For Each item As String In IO.Directory.GetFiles(scripts)
                If allowExts.Contains(IO.Path.GetExtension(item)) Then
                    Me.ScriptsToolStripMenuItem.DropDownItems.Add(IO.Path.GetFileName(item), img, AddressOf OnClickUserScriptsMenuItem)
                    ' AddHandler Me.ScriptsToolStripMenuItem.DropDownItems.Item()
                End If
                Me.ScriptsToolStripMenuItem.Visible = True
            Next
        End If
    End Sub
    Private Sub OnClickUserScriptsMenuItem(sender As Object, e As EventArgs)
        Dim Pr As New Process
        P = Pr
        Pr.Close()
        'P.StartInfo.CreateNoWindow = False
        'P.StartInfo.UseShellExecute = False
        'P.StartInfo.RedirectStandardInput = True
        'P.StartInfo.RedirectStandardOutput = True
        'P.StartInfo.RedirectStandardError = True
        P.StartInfo.FileName = Application.StartupPath() + Me.scripts + sender.Text
        P.StartInfo.Arguments = String.Format("""{0}"" ""{1}""", projectDirPath, projectName)
        P.Start()
    End Sub

    Private Sub MainForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Me.VRPNstate()
    End Sub

    Private Sub PathToVRPNServerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PathToVRPNServerToolStripMenuItem.Click
        'If Not File.Exists(vrpnServerPath) Then
        Me.OpenFileDialog1.FileName = vrpnServerFile
        Me.OpenFileDialog1.InitialDirectory = vrpnServerPath
        If Me.OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim fullPath As String = Me.OpenFileDialog1.FileName
            Dim i As Integer = fullPath.LastIndexOf("\") + 1
            vrpnServerPath = fullPath.Substring(0, i)
            vrpnServerFile = fullPath.Substring(i)
            Me.VRPNstate()
        End If
        'End If
    End Sub

    Private Function VRPNstate() As Boolean
        Dim res As Boolean
        Dim txt1, txt2 As String
        Dim col As Color
        Dim processName As String = vrpnServerFile.Substring(0, vrpnServerFile.IndexOf("."))

        If Not Process.GetProcessesByName(processName).Length > 0 Then
            res = False
            txt1 = "S T O P P E D"
            If File.Exists(vrpnServerPath & vrpnServerFile) Then
                txt2 = " [path to start the server is okay]"
                col = Color.Orange
            Else
                txt2 = " [bad or unknown path to the server]"
                col = Color.Red
            End If
        Else
            res = True
            txt1 = "R U N N I N G"
            txt2 = ""
            col = Color.Lime
        End If

        Me.ToolStripStatusLabel1.Text = txt1
        Me.ToolStripStatusLabel1.BackColor = col
        Me.ToolStripStatusLabel2.Text = txt2
        Return res
    End Function

    Private Sub AutostartOfMasterPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutostartOfMasterPCToolStripMenuItem.Click
        Me.AutostartOfMasterPCToolStripMenuItem.Checked = Not Me.AutostartOfMasterPCToolStripMenuItem.Checked
        Me.AutostartOnMaster = Me.AutostartOfMasterPCToolStripMenuItem.Checked
        Debug.WriteLine("Autostart: {0}", Me.AutostartOnMaster)

        If Me.AutostartOnMaster Then
            Me.ToolStripStatusLabel3.BackColor = Color.Gray
            Me.ToolStripStatusLabel3.ForeColor = Color.LimeGreen
        Else
            Me.ToolStripStatusLabel3.BackColor = Color.FromKnownColor(KnownColor.Control)
            Me.ToolStripStatusLabel3.ForeColor = Color.FromKnownColor(KnownColor.Control)
        End If

    End Sub

    Private Sub ToolStripStatusLabel4_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub AutosaveConfignodexmlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutosaveConfignodexmlToolStripMenuItem.Click
        Me.autoSaveConfigNode = Not Me.autoSaveConfigNode
        Me.AutosaveConfignodexmlToolStripMenuItem.Checked = Me.autoSaveConfigNode
    End Sub

    Private Sub LoadConfigFiles()
        Me.cbConfigNodes.Items.Clear()
        For Each item As String In IO.Directory.GetFiles(projectDirPath + projectName + streamingAssetsDir, "*.xml")
            Me.cbConfigNodes.Items.Add(IO.Path.GetFileName(item))
        Next
        If Me.cbConfigNodes.Items.Count > 0 Then
            Me.cbConfigNodes.Sorted = True
            Me.cbConfigNodes.SelectedIndex = 0
            Me.cbConfigNodes.Enabled = True
            grp_computerList.Enabled = True
            btn_configSave.Enabled = True
            btn_ConfigSaveAs.Enabled = True
            btn_ConfigDelete.Enabled = True
            btn_startProject.Enabled = True
            btn_deployProject.Enabled = True
            btn_updateProject.Enabled = True
            txt_projectname.Text = projectName
            ListBox1.SelectedIndex = 0
        Else
            MsgBox("Configuration file Not found. Please choose correct directory")
        End If
    End Sub

    Private Sub DeleteConfigNode()
        If Me.asNewCnfigNodeSaved Then
            If File.Exists(projectDirPath + projectName + streamingAssetsDir + "node-config.xml") Then
                File.Delete(projectDirPath + projectName + streamingAssetsDir + "node-config.xml")
            End If
        End If
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.DeleteConfigNode()
    End Sub

    Private Sub SaveConfigNodeAs(aCurrConfigPath As String, Optional aSaveAsNewFile As Boolean = False)
        Dim exitLoop As Boolean

        Do
            exitLoop = True
            Me.SaveFileDialog1.FileName = "node-config-" + Date.Now.Ticks.ToString
            Me.SaveFileDialog1.InitialDirectory = projectDirPath + projectName + streamingAssetsDir
            If Me.SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                If Me.SaveFileDialog1.FileName = projectDirPath + projectName + streamingAssetsDir + "node-config.xml" Then
                    MsgBox("Bad name: 'node-config'." + vbNewLine + vbNewLine +
                       "Please use other name for your node configuration file!", MsgBoxStyle.Critical)
                    exitLoop = False
                ElseIf Me.SaveFileDialog1.FileName.Substring(0, Me.SaveFileDialog1.FileName.LastIndexOf("\") + 1) <> projectDirPath + projectName + streamingAssetsDir Then
                    MsgBox("Bad location." + vbNewLine + vbNewLine +
                           "Please save the node configuration file in project path!", MsgBoxStyle.Critical)
                    exitLoop = False
                Else
                    If File.Exists(Me.SaveFileDialog1.FileName) Then
                        File.Delete(Me.SaveFileDialog1.FileName)
                    End If
                    If aSaveAsNewFile Then
                        FileCopy(aCurrConfigPath, Me.SaveFileDialog1.FileName)
                    Else
                        Rename(aCurrConfigPath, Me.SaveFileDialog1.FileName)
                        Me.asNewCnfigNodeSaved = True
                    End If
                End If
            Else
                exitLoop = False
            End If
        Loop Until exitLoop
    End Sub


    Private Sub btn_SaveConfigAs_Click(sender As Object, e As EventArgs) Handles btn_ConfigSaveAs.Click
        Me.SaveConfigNodeAs(Me.currNodeConfigPath, True)
        Me.LoadConfigFiles()
    End Sub

    Private Sub btn_ConfigDelete_Click(sender As Object, e As EventArgs) Handles btn_ConfigDelete.Click
        If MsgBox("Do you realy will remove current node configuration:" + vbNewLine + vbNewLine + "'" + Me.currNodeConfigPath + "'?",
                  MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) Then
            File.Delete(Me.currNodeConfigPath)
            Me.cbConfigNodes.Items.Remove(Me.cbConfigNodes.SelectedItem)
            Me.cbConfigNodes.SelectedIndex = 0
        End If
    End Sub

    'Private Sub KillProjectOnClientsComputersToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    If File.Exists(".\SLAVE_KillUnity.bat") Then
    '        Dim pfd As New ProjectForm

    '        If Application.OpenForms().OfType(Of ProjectForm).Any Then
    '            pf.Close()
    '        End If

    '        DisableButtons()
    '        pf = pfd
    '        pf.txt_projectForm.Text = ""
    '        pf.Text = "Kill Project on Clients' computers"
    '        pf.Visible = True
    '        progressCount = 0
    '        pf.ProgressBar1.Value = 0
    '        pf.ProgressBar1.Maximum = (ListBox1.Items.Count)
    '        opencmd_start()
    '        Dim CMDThread2 As New Threading.Thread(AddressOf CMDConfig)
    '        CMDThread = CMDThread2
    '        'start cmd thread
    '        CMDThread.Start()
    '    Else
    '        MsgBox("SLAVE_KillUnity.bat not found", MsgBoxStyle.Critical, "Not Found")
    '    End If
    'End Sub
End Class