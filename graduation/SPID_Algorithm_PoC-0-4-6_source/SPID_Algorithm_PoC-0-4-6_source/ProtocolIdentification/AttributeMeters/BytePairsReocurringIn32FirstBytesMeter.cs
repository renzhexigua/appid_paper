//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023
    class BytePairsReocurringIn32FirstBytesMeter : IAttributeMeter {

        //private System.Collections.Generic.SortedList<ushort, byte[]> bytePairsFromPreviousPacket;
        private System.Collections.Generic.List<ushort> bytePairsFromPreviousPacket;


        public BytePairsReocurringIn32FirstBytesMeter() {
            //this.bytePairsFromPreviousPacket=new SortedList<ushort, byte[]>();
            this.bytePairsFromPreviousPacket=new List<ushort>();
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "BytePairsReocurringIn32FirstBytesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            System.Collections.Generic.List<ushort> bytePairsInCurrentPacket=new List<ushort>();
            for(int i=0; packetStartIndex+2*i+1<frameData.Length && 2*i+1<packetLength && i<16; i++) {
                ushort key=ConvertToUshort(frameData[packetStartIndex+i*2], frameData[packetStartIndex+i*2+1]);
                if(!bytePairsInCurrentPacket.Contains(key))
                    bytePairsInCurrentPacket.Add(key);
            }

            foreach(ushort key in bytePairsInCurrentPacket)
                if(this.bytePairsFromPreviousPacket.Contains(key))
                    yield return ConvertHelper.ToHashValue((int)key, AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS);
            
            this.bytePairsFromPreviousPacket=bytePairsInCurrentPacket;
            yield break;
        }

        #endregion

        private ushort ConvertToUshort(byte b1, byte b2) {
            ushort returnValue=b1;
            returnValue<<=8;
            returnValue+=b2;
            return returnValue;
        }

    }
}
