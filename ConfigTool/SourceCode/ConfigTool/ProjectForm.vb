Imports System.ComponentModel

Public Class ProjectForm

    Private Sub btn_OK_Click(sender As Object, e As EventArgs) Handles btn_OK.Click
        Close()
    End Sub

    Private Sub ProgressBar1_Validating(sender As Object, e As CancelEventArgs) Handles ProgressBar1.Validating
        'Me.btn_OK.Text = Int(Me.ProgressBar1.Value / Me.ProgressBar1.Maximum).ToString
        'If Me.ProgressBar1.Value = Me.ProgressBar1.Maximum Then
        '    Me.btn_OK.Text = "OK"
        'End If
    End Sub
End Class