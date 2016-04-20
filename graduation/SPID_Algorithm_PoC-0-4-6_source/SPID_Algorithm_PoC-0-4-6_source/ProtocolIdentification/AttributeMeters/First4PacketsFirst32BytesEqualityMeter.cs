//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4PacketsFirst32BytesEqualityMeter : IAttributeMeter {
        #region IClassificationAttribute Members

        byte[] latest32Bytes;

        public string AttributeName {
            get { return "First4PacketsFirst32BytesEqualityMeter"; }
        }

        public First4PacketsFirst32BytesEqualityMeter() {
            //this.firstFiveBytesFromClient=new byte[32];
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                byte[] new32Bytes=new byte[32];
                Array.Copy(frameData, packetStartIndex, new32Bytes, 0, Math.Min(32, frameData.Length-packetStartIndex));
                if(packetOrderNumberInSession>0) {
                    //compare the new bytes to the previous ones
                    System.Collections.BitArray bitArray=new System.Collections.BitArray(32);
                    for(int i=0; i<32; i++)
                        bitArray[i]=(latest32Bytes[i]==new32Bytes[i]);
                    int[] tmpArray=new int[1];
                    bitArray.CopyTo(tmpArray, 0);
                    yield return ConvertHelper.ToHashValue(tmpArray[0], AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS, true);
                }
                this.latest32Bytes=new32Bytes;
            }
            
        }

        #endregion
    }
}