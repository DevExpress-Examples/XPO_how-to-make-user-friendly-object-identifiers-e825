Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo.DB.Exceptions

Namespace UserFriendlyID
	Public NotInheritable Class XpoSequencer
		Inherits XPBaseObject
		' use GUID keys to prepare your database for replication
		<Key(True)> _
		Public Oid As Guid

		<Size(254), Indexed(Unique := True)> _
		Public SequencePrefix As String
		Public Counter As Integer
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Const MaxIdGenerationAttempts As Integer = 7
		Public Const MinConflictDelay As Integer = 50
		Public Const MaxConflictDelay As Integer = 500
		Public Shared Function GetNextValue(ByVal dataLayer As IDataLayer, ByVal sequencePrefix As String) As Integer
			If dataLayer Is Nothing Then
				Throw New ArgumentNullException("dataLayer")
			End If
			If sequencePrefix Is Nothing Then
				sequencePrefix = String.Empty
			End If

			Dim attempt As Integer = 1
			Do
				Try
					Using generatorSession As New Session(dataLayer)
						Dim generator As XpoSequencer = generatorSession.FindObject(Of XpoSequencer)(New OperandProperty("SequencePrefix") = sequencePrefix)
						If generator Is Nothing Then
							generator = New XpoSequencer(generatorSession)
							generator.SequencePrefix = sequencePrefix
						End If
						generator.Counter += 1
						generator.Save()
						Return generator.Counter
					End Using
				Catch e1 As LockingException
					If attempt >= MaxIdGenerationAttempts Then
						Throw
					End If
				End Try
				If attempt > MaxIdGenerationAttempts \ 2 Then
					Thread.Sleep(New Random().Next(MinConflictDelay, MaxConflictDelay))
				End If

				attempt += 1
			Loop
		End Function
		Public Shared Function GetNextValue(ByVal sequencePrefix As String) As Integer
			Return GetNextValue(XpoDefault.DataLayer, sequencePrefix)
		End Function
	End Class
End Namespace
