Imports System.IO
Imports Newtonsoft.Json.Linq
Imports IBM.Data.DB2


Module Program
	Sub Main(args As String())
		Dim STR As StreamReader
		STR = New StreamReader("config.json")
		Dim json As String = STR.ReadToEnd
		Dim jsonObject As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.Linq.JObject.Parse(json)

		Console.WriteLine(jsonObject.Item("queries")(0)("file"))
		ReadEdumateData(jsonObject)
	End Sub


	Sub ReadEdumateData(jsonObject As Newtonsoft.Json.Linq.JObject)

		Using conn As New IBM.Data.DB2.DB2Connection(jsonObject.Item("edumateConnectionstring"))
			conn.Open()

			For Each item In jsonObject.Item("queries")
				Dim sw As New StreamWriter(item("file"))
				Dim command As New IBM.Data.DB2.DB2Command(item("query"), conn)
				command.Connection = conn
				command.CommandText = item("query")
				Dim dr As IBM.Data.DB2.DB2DataReader
				dr = command.ExecuteReader

				Dim outRow As String = ""

				For i As Integer = 0 To dr.FieldCount - 1
					Try
						outRow = outRow & dr.GetName(i) & ","
					Catch
					End Try
				Next

				sw.WriteLine(outRow.Substring(0, outRow.Length - 1))
				Console.WriteLine((outRow.Substring(0, outRow.Length - 1)))

				While dr.Read()
					outRow = ""
					If Not dr.IsDBNull(0) Then
						For i As Integer = 0 To dr.FieldCount - 1
							outRow = outRow & dr.GetValue(i) & ","
						Next
						sw.WriteLine(outRow.Substring(0, outRow.Length - 1))
						Console.WriteLine((outRow.Substring(0, outRow.Length - 1)))
					End If
				End While
			Next

		End Using

	End Sub


End Module
