using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Exceptions;

namespace UserFriendlyID {
    public sealed class XpoSequencer : XPBaseObject {
        // use GUID keys to prepare your database for replication
        [Key(true)]
        public Guid Oid;
        
        [Size(254), Indexed(Unique = true)]
        public string SequencePrefix;
        public int Counter;
        public XpoSequencer(Session session) : base(session) { }
        public const int MaxIdGenerationAttempts = 7;
        public const int MinConflictDelay = 50;
        public const int MaxConflictDelay = 500;
        public static int GetNextValue(IDataLayer dataLayer, string sequencePrefix) {
            if(dataLayer == null)
                throw new ArgumentNullException("dataLayer");
            if(sequencePrefix == null)
                sequencePrefix = string.Empty;

            int attempt = 1;
            while(true) {
                try {
                    using(Session generatorSession = new Session(dataLayer)) {
                        XpoSequencer generator = generatorSession.FindObject<XpoSequencer>(new OperandProperty("SequencePrefix") == sequencePrefix);
                        if(generator == null) {
                            generator = new XpoSequencer(generatorSession);
                            generator.SequencePrefix = sequencePrefix;
                        }
                        generator.Counter++;
                        generator.Save();
                        return generator.Counter;
                    }
                }
                catch(LockingException) {
                    if(attempt >= MaxIdGenerationAttempts)
                        throw;
                }
                if(attempt > MaxIdGenerationAttempts / 2)
                    Thread.Sleep(new Random().Next(MinConflictDelay, MaxConflictDelay));

                attempt++;
            }
        }
        public static int GetNextValue(string sequencePrefix) {
            return GetNextValue(XpoDefault.DataLayer, sequencePrefix);
        }
    }
}
