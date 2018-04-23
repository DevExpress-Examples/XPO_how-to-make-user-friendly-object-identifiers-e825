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
	Public NotInheritable Class XpoServerId
		Inherits XPLiteObject
		Private Shared SyncRoot As Object = New Object()
		<Key(False)> _
		Public Property Zero() As Integer
			Get
				Return 0
			End Get
			Set(ByVal value As Integer)
				If value <> 0 Then
					Throw New ArgumentException("0 expected")
				End If
			End Set
		End Property
		Public SequencePrefix As String
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Private Shared cachedSequencePrefix As String = Nothing
		'static WeakReference dataLayerForCachedServerPrefix = new WeakReference(null);
		Private Shared dataLayerForCachedServerPrefix As IDataLayer = Nothing
		Public Shared Sub ResetCache()
			dataLayerForCachedServerPrefix = Nothing
			' dataLayerForCachedServerPrefix.Target = null; <<< if WeakReference
		End Sub
		Public Shared Function GetSequencePrefix(ByVal dataLayer As IDataLayer) As String
			If dataLayer Is Nothing Then
				Throw New ArgumentNullException("dataLayer")
			End If
			SyncLock SyncRoot
'INSTANT VB NOTE: Embedded comments are not maintained by Instant VB
'ORIGINAL LINE: if(dataLayerForCachedServerPrefix/*.Target*/ != dataLayer) {
				If dataLayerForCachedServerPrefix IsNot dataLayer Then
					Using session As New Session(dataLayer)
						Dim sid As XpoServerId = session.GetObjectByKey(Of XpoServerId)(0)
						If sid Is Nothing Then
							' we can throw exception here instead of creating random prefix
							sid = New XpoServerId(session)
							sid.SequencePrefix = XpoDefault.NewGuid().ToString()
							Try
								sid.Save()
							Catch
								sid = session.GetObjectByKey(Of XpoServerId)(0, True)
								If sid Is Nothing Then
									Throw
								End If
							End Try
						End If
						cachedSequencePrefix = sid.SequencePrefix
						dataLayerForCachedServerPrefix = dataLayer
						' dataLayerForCachedServerPrefix.Target = dataLayer; <<< if WeakReference
					End Using
				End If
				Return cachedSequencePrefix
			End SyncLock
		End Function
		Public Shared Function GetNextUniqueValue(ByVal dataLayer As IDataLayer, ByVal sequencePrefix As String) As Integer
			If dataLayer Is Nothing Then
				Throw New ArgumentNullException("dataLayer")
			End If
			Dim realSeqPrefix As String = sequencePrefix & "@"c + GetSequencePrefix(dataLayer)
			Return XpoSequencer.GetNextValue(dataLayer, realSeqPrefix)
		End Function
		Public Shared Function GetNextUniqueValue(ByVal sequencePrefix As String) As Integer
			Return GetNextUniqueValue(XpoDefault.DataLayer, sequencePrefix)
		End Function
	End Class
End Namespace
