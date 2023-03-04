using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class SimplexNoise {
    private const int GradientSizeTable = 256;
    private static readonly int[] _perm = new int[GradientSizeTable * 2];
    private static readonly float[] _grad = new float[GradientSizeTable * 3];

    static SimplexNoise() {
        var rand = new Random();
        for (var i = 0; i < GradientSizeTable; i++) {
            while (true) {
                var n = rand.Next(GradientSizeTable);
                if (!Array.Exists(_perm, x => x == n)) {
                    _perm[i] = n;
                    _perm[i + GradientSizeTable] = n;
                    break;
                }
            }

            var angle = rand.NextDouble() * 2 * Math.PI;
            _grad[i * 3] = (float)Math.Cos(angle);
            _grad[i * 3 + 1] = (float)Math.Sin(angle);
            _grad[i * 3 + 2] = (float)Math.Cos(angle);
        }
    }

    public static float Generate(float x, float y, float frequency, int octaves, float lacunarity, float persistence) {
        var amplitude = 1f;
        var max = 1f;
        var result = 0f;

        for (var i = 0; i < octaves; i++) {
            result += Generate(x * frequency, y * frequency) * amplitude;
            frequency *= lacunarity;
            max += amplitude;
            amplitude *= persistence;
        }

        return result / max;
    }

    private static float Generate(float x, float y) {
        var s = (x + y) * 0.366025403784439f; // F4 = (sqrt(5) - 1) / 4
        var i = FastFloor(x + s);
        var j = FastFloor(y + s);

        var t = (i + j) * 0.211324865405187f; // G4 = (5 - sqrt(5)) / 20
        var x0 = x - i + t;
        var y0 = y - j + t;

        int i1, j1;
        if (x0 > y0) {
            i1 = 1; j1 = 0;
        }
        else {
            i1 = 0; j1 = 1;
        }

        var x1 = x0 - i1 + t;
        var y1 = y0 - j1 + t;
        var x2 = x0 - 1 + 2 * 0.211324865405187f + t;
        var y2 = y0 - 1 + 2 * 0.211324865405187f + t;

        var ii = i & 255;
        var jj = j & 255;

        var gi0 = _perm[ii + _perm[jj]] % 12;
        var gi1 = _perm[ii + i1 + _perm[jj + j1]] % 12;
        var gi2 = _perm[ii + 1 + _perm[jj + 1]] % 12;

        var n0 = Dot(_grad, gi0, x0, y0);
        var n1 = Dot(_grad, gi1, x1, y1);
        var n2 = Dot(_grad, gi2, x2, y2);

        return 70 * (n0 + n1 + n2);
    }

    private static int FastFloor(float x) {
        return x > 0 ? (int)x : (int)x - 1;
    }

    private static float Dot(float[] grad, int index, float x, float y) {
        return grad[index * 3] * x + grad[index * 3 + 1] * y;
    }
}