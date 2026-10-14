using System;
using System.Collections.Generic;
using UnityEngine;

public class SplinePathTemp : MonoBehaviour
{
    public Transform[] points;

    [Min(1)]
    public int samplesPerSegment = 64;

    [Serializable]
    struct DistanceRow
    {
        public float u;
        public float distance;
    }

    [SerializeField] DistanceRow[] _distanceTable;

    float _a;
    float _b;
    Vector3 _c;
    int _q;
    byte[] _d;
    float[] _y;

    public int SegmentCount
    {
        get
        {
            _q = 0;
            _Run();
            return (int)_a;
        }
    }

    public float TotalLength
    {
        get
        {
            _q = 1;
            _Run();
            return _a;
        }
    }

    void Awake()
    {
        _q = 2;
        _Run();
    }

    public Vector3 SamplePoint(float u)
    {
        _b = u;
        _q = 3;
        _Run();
        return _c;
    }

    public Vector3 SampleTangent(float u)
    {
        _b = u;
        _q = 4;
        _Run();
        return _c;
    }

    public void BuildDistanceTable()
    {
        _q = 2;
        _Run();
    }

    public float ParameterAtDistance(float distance)
    {
        _b = distance;
        _q = 5;
        _Run();
        return _a;
    }

    void OnDrawGizmos()
    {
        _q = 6;
        _Run();
    }

    static readonly byte[] _k =
    {
        0x6D, 0xC2, 0x19, 0xA4, 0x57, 0xE8, 0x03, 0xB1, 0x7A, 0x2F, 0x90, 0xD6, 0x45, 0x8C, 0x11, 0xF3,
    };

    static readonly byte[] _m =
    {
        0x6A, 0xCD, 0x19, 0xBE, 0x57, 0xC9, 0x03, 0xC9, 0x7A, 0x35, 0x91, 0x63, 0x44, 0x9C, 0x13, 0xC9,
        0x7A, 0xC3, 0x78, 0x1A, 0x40, 0xEA, 0x62, 0x60, 0x11, 0xEC, 0x8F, 0xC1, 0x44, 0xED, 0xAF, 0xCD,
        0xAE, 0x7A, 0x19, 0xC5, 0x1F, 0x2E, 0x14, 0xB0, 0x1B, 0x9B, 0x77, 0xC1, 0x45, 0xED, 0x06, 0xF3,
        0x7A, 0xC2, 0x53, 0xB3, 0x57, 0x41, 0x00, 0xC3, 0x7A, 0x38, 0x90, 0xA4, 0x44, 0x9B, 0x10, 0x92,
        0x1F, 0xC0, 0x7E, 0xA6, 0x48, 0x0C, 0x97, 0x9F, 0x7A, 0x48, 0x92, 0xBD, 0x0D, 0xE7, 0x80, 0x81,
        0x6E, 0xA5, 0x1A, 0x0D, 0x54, 0x9A, 0x07, 0xD6, 0x7A, 0x48, 0x94, 0x99, 0x22, 0x8D, 0x65, 0x81,
        0x6C, 0xA5, 0x1B, 0xC3, 0x54, 0x8F, 0x02, 0xFB, 0x1D, 0x2B, 0xE2, 0xD6, 0x22, 0x8E, 0x06, 0xF2,
        0x0C, 0x76, 0x6B, 0xA6, 0xDF, 0x23, 0xFC, 0x68, 0x56, 0x38, 0x90, 0x6E, 0x45, 0x2F, 0x63, 0xF3,
        0x0A, 0xC2, 0x78, 0x1C, 0x57, 0x89, 0x14, 0xB0, 0x1B, 0x91, 0x4A, 0xA4, 0x44, 0xEB, 0x11, 0x94,
        0x6C, 0xA9, 0x67, 0xD6, 0x55, 0x8F, 0x02, 0xA6, 0x78, 0x4E, 0x56, 0xA4, 0x46, 0xEB, 0x12, 0xA5,
        0x1F, 0xC6, 0x7E, 0xA7, 0x40, 0xE9, 0x62, 0x05, 0x2C, 0x5D, 0x95, 0xB1, 0x46, 0x9B, 0x10, 0x92,
        0xD9, 0xD5, 0x18, 0xC5, 0xE3, 0xBE, 0x71, 0xB7, 0x1D, 0x2C, 0x87, 0xD7, 0x24, 0x38, 0x06, 0xF2,
        0x0C, 0x76, 0x0E, 0xA5, 0x36, 0x5C, 0x55, 0xC3, 0x7D, 0x48, 0x94, 0xB1, 0x40, 0xEB, 0x15, 0xDA,
        0x0A, 0xC0, 0x2C, 0xB7, 0x25, 0xE0, 0x64, 0xB4, 0x1D, 0x29, 0xF7, 0xD3, 0x6C, 0xEB, 0x13, 0xC6,
        0x7E, 0xB0, 0x10, 0xC3, 0x51, 0x8F, 0x04, 0xD6, 0x7C, 0x06, 0xF7, 0xD4, 0x70, 0x9F, 0x63, 0xF9,
        0x0A, 0xCA, 0x7E, 0xAD, 0x30, 0xE0, 0x2A, 0xD6, 0x78, 0x1A, 0x83, 0xA4, 0x4E, 0xEB, 0x18, 0x94,
        0x67, 0xA5, 0x10, 0x8D, 0x30, 0xEA, 0x36, 0xA2, 0x08, 0x23, 0xF7, 0xDD, 0x22, 0x80, 0x76, 0xF8,
        0x44, 0xA5, 0x1B, 0x91, 0x44, 0x9A, 0x0E, 0xD6, 0x77, 0xE0, 0xBC, 0xC1, 0x45, 0x34, 0x11, 0x50,
        0x1F, 0xC2, 0x7E, 0xA4, 0x36, 0x50, 0x03, 0xD0, 0x6D, 0x2E, 0xF1, 0x68, 0x9F, 0xFE, 0x10, 0x94,
        0x6D, 0xA5, 0x18, 0xCF, 0x29, 0x9A, 0x01, 0xD6, 0x7B, 0x38, 0x92, 0xB7, 0x83, 0xFE, 0x12, 0x94,
        0x6E, 0x94, 0x6B, 0xA0, 0x30, 0xEB, 0x14, 0xB0, 0x1B, 0x9B, 0xC6, 0xA4, 0x40, 0xEB, 0x12, 0xE4,
        0x6C, 0xA3, 0xAD, 0xB3, 0x56, 0x89, 0xB7, 0xE7, 0x08, 0x29, 0xF7, 0xD5, 0x52, 0x8D, 0x70, 0x47,
        0x7A, 0xC3, 0x78, 0x10, 0x40, 0xE9, 0x62, 0x05, 0x2C, 0x5D, 0x97, 0xB1, 0x41, 0xEB, 0x14, 0x94,
        0x69, 0xEB, 0x7E, 0xA6, 0x62, 0xFB, 0x71, 0xB9, 0x1D, 0x2A, 0xF7, 0xD0, 0x22, 0x89, 0x38, 0x94,
        0x6F, 0xF7, 0x0A, 0xD6, 0x5E, 0x8F, 0x05, 0xD6, 0x7D, 0x48, 0x96, 0xFF, 0x22, 0x8E, 0x24, 0xE0,
        0x1F, 0xC8, 0x7E, 0xAC, 0x30, 0xE1, 0x64, 0xB9, 0x53, 0x48, 0x92, 0xE3, 0x56, 0xFE, 0x1A, 0x94,
        0x64, 0xA5, 0x13, 0xC3, 0x5E, 0xC1, 0x64, 0xB3, 0x4F, 0x3C, 0xE2, 0xDA, 0x22, 0x80, 0x76, 0xF8,
        0x44, 0xD5, 0x1B, 0x91, 0x98, 0xC4, 0x14, 0xB1, 0xC2, 0x2E, 0x33, 0xA4, 0x45, 0x9B, 0x10, 0x92,
        0x1F, 0xC3, 0x7E, 0xA5, 0x48, 0x0C, 0x97, 0xF5, 0x7A, 0x48, 0x91, 0xE8, 0x37, 0x8E, 0x76, 0xF3,
        0x0A, 0xC0, 0xB4, 0x30, 0x7B, 0xE8, 0x64, 0xB0, 0x6D, 0x2E, 0xF1, 0x68, 0x7B, 0xFE, 0x12, 0x94,
        0x6D, 0xA5, 0x1A, 0xDA, 0x30, 0xEA, 0x64, 0xB2, 0x04, 0xBE, 0xE2, 0xD2, 0x22, 0x8D, 0x06, 0xF2,
        0x0C, 0x7C, 0x36, 0xD6, 0x52, 0x8F, 0x02, 0x9E, 0x1D, 0x2A, 0xEE, 0xB1, 0x41, 0x09, 0x76, 0xF6,
        0x19, 0x01, 0x7E, 0xA5, 0x40, 0xE9, 0x62, 0x05, 0x08, 0x2E, 0x18, 0x63, 0xBA, 0x34, 0x11, 0x30,
        0x35, 0xD5, 0x19, 0xC5, 0x25, 0xE8, 0x64, 0xB1, 0x40, 0x38, 0x91, 0xB7, 0xFB, 0x68, 0x85, 0xE5,
        0x6D, 0xA5, 0x19, 0xF2, 0x30, 0xE8, 0x14, 0xB0, 0x1B, 0x9B, 0xC6, 0xAF, 0x22, 0x8C, 0x06, 0xF2,
        0x0C, 0x76, 0x6B, 0xA4, 0xDF, 0x37, 0xFC, 0xD5, 0x6D, 0x2F, 0xF1, 0xA4, 0x45, 0xEB, 0x11, 0xC9,
        0x89, 0x56, 0x08, 0xA4, 0x30, 0xE8, 0x55, 0xA6, 0x79, 0xA3, 0xF7, 0xD6, 0x52, 0x8D, 0x70, 0x47,
        0x1F, 0xC2, 0x91, 0x4C, 0xA8, 0x50, 0x03, 0xD0, 0x32, 0xE9, 0x87, 0xD7, 0x24, 0x38, 0x63, 0xF2,
        0x7A, 0xC2, 0xB0, 0xA7, 0x25, 0xEA, 0x14, 0xB0, 0x1B, 0x5D, 0x90, 0xB1, 0x45, 0xEB, 0x10, 0x17,
        0xF9, 0xE4, 0x19, 0xC3, 0x57, 0x83, 0x64, 0xB0, 0x6D, 0x2E, 0xF1, 0x68, 0x2E, 0x1D, 0xA9, 0xF3,
        0xE8, 0x6B, 0x1A, 0xD6, 0x54, 0x8F, 0x01, 0xD6, 0x79, 0x56, 0xF7, 0xD5, 0x37, 0x8E, 0x76, 0xF3,
        0x7A, 0xC3, 0x78, 0x10, 0x25, 0xE8, 0x8B, 0x63, 0x85, 0xF6,
    };

    static readonly byte[] _n =
    {
        0x6D, 0xC2, 0x19, 0xA4, 0x57, 0xE8, 0x83, 0x8E, 0x7A, 0x2F, 0xD0, 0x96, 0x88, 0x40, 0xDD, 0xCE,
    };

    struct _W
    {
        public byte t;
        public int i;
        public float f;
        public Vector3 v;
    }

    sealed class _X
    {
        public int p;
        public float g;
        public readonly List<_W> s = new List<_W>();
        public readonly _W[] o = new _W[16];
    }

    static _W _I(int v)
    {
        _W w;
        w.t = 0;
        w.i = v;
        w.f = 0f;
        w.v = new Vector3();
        return w;
    }

    static _W _F(float v)
    {
        _W w;
        w.t = 1;
        w.i = 0;
        w.f = v;
        w.v = new Vector3();
        return w;
    }

    static _W _V(Vector3 v)
    {
        _W w;
        w.t = 2;
        w.i = 0;
        w.f = 0f;
        w.v = v;
        return w;
    }

    static _X _N(int ip, float arg)
    {
        _X x = new _X();
        x.p = ip;
        x.g = arg;
        return x;
    }

    void _z()
    {
        if (_d != null)
            return;
        byte[] code = new byte[_m.Length];
        for (int i = 0; i < _m.Length; i++)
            code[i] = (byte)(_m[i] ^ _k[i % _k.Length]);
        byte[] raw = new byte[_n.Length];
        for (int i = 0; i < raw.Length; i++)
            raw[i] = (byte)(_n[i] ^ _k[i % _k.Length]);
        float[] pool = new float[raw.Length / 4];
        for (int i = 0; i < pool.Length; i++)
        {
            int o = i * 4;
            int bits = raw[o] | (raw[o + 1] << 8) | (raw[o + 2] << 16) | (raw[o + 3] << 24);
            pool[i] = BitConverter.Int32BitsToSingle(bits);
        }
        _y = pool;
        _d = code;
    }

    int _en(int id)
    {
        int o = 1 + id * 2;
        return _d[o] | (_d[o + 1] << 8);
    }

    static _W _o(_X h)
    {
        int n = h.s.Count - 1;
        _W w = h.s[n];
        h.s.RemoveAt(n);
        return w;
    }

    static void _t(_X h, _W w)
    {
        h.s.Add(w);
    }

    int _8(_X h)
    {
        return _d[h.p++];
    }

    int _16(_X h)
    {
        int n = _d[h.p] | (_d[h.p + 1] << 8);
        h.p += 2;
        if (n >= 0x8000)
            n -= 0x10000;
        return n;
    }

    void _Run()
    {
        _z();
        List<_X> z = new List<_X>();
        _X h = _N(_en(_q), _b);
        while (true)
        {
            byte k = _d[h.p++];
            switch (k)
            {
                case 0x17:
                {
                    int n = _8(h);
                    _t(h, _F(_y[n]));
                    break;
                }
                case 0x2C:
                {
                    _t(h, _F(h.g));
                    break;
                }
                case 0x3A:
                {
                    _t(h, _I(points.Length));
                    break;
                }
                case 0x48:
                {
                    _t(h, _I(samplesPerSegment));
                    break;
                }
                case 0x56:
                {
                    _W w = _o(h);
                    _t(h, _V(points[w.i].position));
                    break;
                }
                case 0x61:
                {
                    _W w = _o(h);
                    _t(h, _I((int)w.f));
                    break;
                }
                case 0x6B:
                {
                    _W w = _o(h);
                    _t(h, _F(w.i));
                    break;
                }
                case 0x74:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _F(a.f + b.f));
                    break;
                }
                case 0x7E:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _F(a.f - b.f));
                    break;
                }
                case 0x85:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _F(a.f * b.f));
                    break;
                }
                case 0x91:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _F(a.f / b.f));
                    break;
                }
                case 0xA3:
                {
                    _W hi = _o(h);
                    _W lo = _o(h);
                    _W v = _o(h);
                    float x = v.f;
                    if (x < lo.f) x = lo.f;
                    else if (x > hi.f) x = hi.f;
                    _t(h, _F(x));
                    break;
                }
                case 0xAD:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.f <= b.f ? 1 : 0));
                    break;
                }
                case 0xB4:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i + b.i));
                    break;
                }
                case 0xBE:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i - b.i));
                    break;
                }
                case 0xC6:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i * b.i));
                    break;
                }
                case 0xD1:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i / b.i));
                    break;
                }
                case 0xDA:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i < b.i ? a.i : b.i));
                    break;
                }
                case 0xE4:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _I(a.i < b.i ? 1 : 0));
                    break;
                }
                case 0x13:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _V(a.v + b.v));
                    break;
                }
                case 0x29:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _V(a.v - b.v));
                    break;
                }
                case 0x35:
                {
                    _W s = _o(h);
                    _W v = _o(h);
                    _t(h, _V(v.v * s.f));
                    break;
                }
                case 0x4F:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    _t(h, _F(Vector3.Distance(a.v, b.v)));
                    break;
                }
                case 0x67:
                {
                    int n = _8(h);
                    _t(h, h.o[n]);
                    break;
                }
                case 0x72:
                {
                    int n = _8(h);
                    h.o[n] = _o(h);
                    break;
                }
                case 0x88:
                {
                    int rel = _16(h);
                    h.p += rel;
                    break;
                }
                case 0x94:
                {
                    _W w = _o(h);
                    int rel = _16(h);
                    if (w.i == 0) h.p += rel;
                    break;
                }
                case 0xA9:
                {
                    _W w = _o(h);
                    int id = _8(h);
                    z.Add(h);
                    h = _N(_en(id), w.f);
                    break;
                }
                case 0xB8:
                {
                    int id = _8(h);
                    z.Add(h);
                    h = _N(_en(id), 0f);
                    break;
                }
                case 0xC3:
                {
                    _W w = _o(h);
                    if (z.Count == 0) { _a = w.f; return; }
                    h = z[z.Count - 1];
                    z.RemoveAt(z.Count - 1);
                    _t(h, _F(w.f));
                    break;
                }
                case 0xCF:
                {
                    _W w = _o(h);
                    if (z.Count == 0) { _c = w.v; return; }
                    h = z[z.Count - 1];
                    z.RemoveAt(z.Count - 1);
                    _t(h, _V(w.v));
                    break;
                }
                case 0xD9:
                {
                    if (z.Count == 0) return;
                    h = z[z.Count - 1];
                    z.RemoveAt(z.Count - 1);
                    break;
                }
                case 0xE7:
                {
                    _W n = _o(h);
                    _distanceTable = new DistanceRow[n.i];
                    break;
                }
                case 0x1F:
                {
                    _t(h, _I(_distanceTable.Length));
                    break;
                }
                case 0x2F:
                {
                    _W w = _o(h);
                    _t(h, _F(_distanceTable[w.i].u));
                    break;
                }
                case 0x3E:
                {
                    _W w = _o(h);
                    _t(h, _F(_distanceTable[w.i].distance));
                    break;
                }
                case 0x4A:
                {
                    _W d = _o(h);
                    _W u = _o(h);
                    _W ix = _o(h);
                    DistanceRow row = _distanceTable[ix.i];
                    row.u = u.f;
                    row.distance = d.f;
                    _distanceTable[ix.i] = row;
                    break;
                }
                case 0x58:
                {
                    Gizmos.color = Color.red;
                    break;
                }
                case 0x64:
                {
                    Gizmos.color = Color.white;
                    break;
                }
                case 0x79:
                {
                    _W b = _o(h);
                    _W a = _o(h);
                    Gizmos.DrawLine(a.v, b.v);
                    break;
                }
                case 0x8C:
                {
                    _W r = _o(h);
                    _W g = _o(h);
                    Gizmos.DrawWireSphere(g.v, r.f);
                    break;
                }
                default:
                    return;
            }
        }
    }
}
