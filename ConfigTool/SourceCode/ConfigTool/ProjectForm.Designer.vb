﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.btn_OK = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txt_projectForm = New System.Windows.Forms.RichTextBox()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 377)
        Me.ProgressBar1.Maximum = 320
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(724, 23)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.TabIndex = 0
        Me.ProgressBar1.Value = 200
        '
        'btn_OK
        '
        Me.btn_OK.Enabled = False
        Me.btn_OK.Location = New System.Drawing.Point(742, 377)
        Me.btn_OK.Name = "btn_OK"
        Me.btn_OK.Size = New System.Drawing.Size(70, 23)
        Me.btn_OK.TabIndex = 1
        Me.btn_OK.Text = "OK"
        Me.btn_OK.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Panel1.Controls.Add(Me.txt_projectForm)
        Me.Panel1.Controls.Add(Me.ProgressBar1)
        Me.Panel1.Controls.Add(Me.btn_OK)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(824, 412)
        Me.Panel1.TabIndex = 3
        '
        'txt_projectForm
        '
        Me.txt_projectForm.Dock = System.Windows.Forms.DockStyle.Top
        Me.txt_projectForm.Font = New System.Drawing.Font("Courier New", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt_projectForm.Location = New System.Drawing.Point(0, 0)
        Me.txt_projectForm.Name = "txt_projectForm"
        Me.txt_projectForm.ReadOnly = True
        Me.txt_projectForm.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.txt_projectForm.Size = New System.Drawing.Size(824, 357)
        Me.txt_projectForm.TabIndex = 3
        Me.txt_projectForm.Text = " 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 " &
    "123456789 123456789 "
        '
        'ProjectForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(824, 412)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.MaximumSize = New System.Drawing.Size(840, 450)
        Me.MinimumSize = New System.Drawing.Size(840, 450)
        Me.Name = "ProjectForm"
        Me.Text = "ProjectForm"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents btn_OK As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents txt_projectForm As RichTextBox
End Class
