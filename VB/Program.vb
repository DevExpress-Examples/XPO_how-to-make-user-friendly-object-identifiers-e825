Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Data.Filtering
Imports System.Threading
Imports DevExpress.Xpo.DB.Exceptions

Namespace UserFriendlyID
	Friend Class Program
		Private Shared Sub [Do](ByVal seqPrefix As String)
			Dim nextUniqueValue As Integer = XpoServerId.GetNextUniqueValue(seqPrefix)
			Console.WriteLine("seq '{0}': {1}", seqPrefix, nextUniqueValue)
		End Sub
		Shared Sub Main(ByVal args() As String)
			XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema)
			[Do]("A")
			[Do]("A")
			[Do]("B")
			[Do]("A")
			Console.ReadLine()
		End Sub
	End Class
End Namespace
