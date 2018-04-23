using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using System.Threading;
using DevExpress.Xpo.DB.Exceptions;

namespace UserFriendlyID {
	class Program {
        static void Do(string seqPrefix) {
            int nextUniqueValue = XpoServerId.GetNextUniqueValue(seqPrefix);
            Console.WriteLine("seq '{0}': {1}", seqPrefix, nextUniqueValue);
        }
		static void Main(string[] args) {
			XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema);
			Do("A");
			Do("A");
			Do("B");
			Do("A");
            Console.ReadLine();
		}
	}
}
