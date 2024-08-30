using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CombatReportScripts
{
    [Serializable]
    public struct CombatReportData : INetworkSerializable
    {
        [SerializeField]
        public ulong playerID;
        
        [SerializeField]
        public FixedString64Bytes playerName;
        
        [SerializeField]
        public Color playerColor;

        [SerializeField]
        public int reliquiasColetadas;

        [SerializeField]
        public int itensColetados;

        [SerializeField]
        public int dinheiroRecebido;

        [SerializeField]
        public int vezesPreso;

        [SerializeField]
        public float distanciaPercorrida;

        [SerializeField]
        public int itensUsados;

        [SerializeField]
        public int vezesAtacado;
        
        public override string ToString()
        {
            return
                $"Reliquias coletadas: {reliquiasColetadas}, Itens coletados: {itensColetados}, Dinheiro recebido: {dinheiroRecebido}, Vezes preso: {vezesPreso}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerColor);
            serializer.SerializeValue(ref reliquiasColetadas);
            serializer.SerializeValue(ref itensColetados);
            serializer.SerializeValue(ref dinheiroRecebido);
            serializer.SerializeValue(ref vezesPreso);
            serializer.SerializeValue(ref distanciaPercorrida);
            serializer.SerializeValue(ref itensUsados);
            serializer.SerializeValue(ref vezesAtacado);
        }
    }
}