<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class froggLogo
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(froggLogo))
        Me.froggLogoImg = New System.Windows.Forms.PictureBox()
        CType(Me.froggLogoImg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'froggLogoImg
        '
        Me.froggLogoImg.Image = Global.FroggPTLauncher.My.Resources.Resources.frogglogo
        Me.froggLogoImg.Location = New System.Drawing.Point(0, 0)
        Me.froggLogoImg.Name = "froggLogoImg"
        Me.froggLogoImg.Size = New System.Drawing.Size(479, 152)
        Me.froggLogoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.froggLogoImg.TabIndex = 0
        Me.froggLogoImg.TabStop = False
        '
        'froggLogo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(479, 152)
        Me.Controls.Add(Me.froggLogoImg)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "froggLogo"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Frogg Copyright"
        Me.TransparencyKey = System.Drawing.SystemColors.ActiveCaption
        CType(Me.froggLogoImg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents froggLogoImg As System.Windows.Forms.PictureBox
End Class
