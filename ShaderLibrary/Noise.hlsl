#ifndef RADISH_NOISE_H
#define RADISH_NOISE_H

/*
Copyright (C) 2011 by Ashima Arts (Simplex noise)
Copyright (C) 2011-2016 by Stefan Gustavson (Classic noise and others)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

float wglnoise_mod(float x, float y)
{
    return x - y * floor(x / y);
}

float2 wglnoise_mod(float2 x, float2 y)
{
    return x - y * floor(x / y);
}

float3 wglnoise_mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}

float4 wglnoise_mod(float4 x, float4 y)
{
    return x - y * floor(x / y);
}

float2 wglnoise_fade(float2 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float3 wglnoise_fade(float3 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float wglnoise_mod289(float x)
{
    return x - floor(x / 289) * 289;
}

float2 wglnoise_mod289(float2 x)
{
    return x - floor(x / 289) * 289;
}

float3 wglnoise_mod289(float3 x)
{
    return x - floor(x / 289) * 289;
}

float4 wglnoise_mod289(float4 x)
{
    return x - floor(x / 289) * 289;
}

float3 wglnoise_permute(float3 x)
{
    return wglnoise_mod289((x * 34 + 1) * x);
}

float4 wglnoise_permute(float4 x)
{
    return wglnoise_mod289((x * 34 + 1) * x);
}

float ClassicNoise_impl(float2 pi0, float2 pf0, float2 pi1, float2 pf1)
{
    pi0 = wglnoise_mod289(pi0); // To avoid truncation effects in permutation
    pi1 = wglnoise_mod289(pi1);

    float4 ix = float2(pi0.x, pi1.x).xyxy;
    float4 iy = float2(pi0.y, pi1.y).xxyy;
    float4 fx = float2(pf0.x, pf1.x).xyxy;
    float4 fy = float2(pf0.y, pf1.y).xxyy;

    float4 i = wglnoise_permute(wglnoise_permute(ix) + iy);

    float4 phi = i / 41 * 3.14159265359 * 2;
    float2 g00 = float2(cos(phi.x), sin(phi.x));
    float2 g10 = float2(cos(phi.y), sin(phi.y));
    float2 g01 = float2(cos(phi.z), sin(phi.z));
    float2 g11 = float2(cos(phi.w), sin(phi.w));

    float n00 = dot(g00, float2(fx.x, fy.x));
    float n10 = dot(g10, float2(fx.y, fy.y));
    float n01 = dot(g01, float2(fx.z, fy.z));
    float n11 = dot(g11, float2(fx.w, fy.w));

    float2 fade_xy = wglnoise_fade(pf0);
    float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
    float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
    return 1.44 * n_xy;
}

// Classic Perlin noise
float ClassicNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    return ClassicNoise_impl(i, f, i + 1, f - 1);
}

// Classic Perlin noise, periodic variant
float PeriodicNoise(float2 p, float2 rep)
{
    float2 i0 = wglnoise_mod(floor(p), rep);
    float2 i1 = wglnoise_mod(i0 + 1, rep);
    float2 f = frac(p);
    return ClassicNoise_impl(i0, f, i1, f - 1);
}

float ClassicNoise_impl(float3 pi0, float3 pf0, float3 pi1, float3 pf1)
{
    pi0 = wglnoise_mod289(pi0);
    pi1 = wglnoise_mod289(pi1);

    float4 ix = float4(pi0.x, pi1.x, pi0.x, pi1.x);
    float4 iy = float4(pi0.y, pi0.y, pi1.y, pi1.y);
    float4 iz0 = pi0.z;
    float4 iz1 = pi1.z;

    float4 ixy = wglnoise_permute(wglnoise_permute(ix) + iy);
    float4 ixy0 = wglnoise_permute(ixy + iz0);
    float4 ixy1 = wglnoise_permute(ixy + iz1);

    float4 gx0 = lerp(-1, 1, frac(floor(ixy0 / 7) / 7));
    float4 gy0 = lerp(-1, 1, frac(floor(ixy0 % 7) / 7));
    float4 gz0 = 1 - abs(gx0) - abs(gy0);

    bool4 zn0 = gz0 < -0.01;
    gx0 += zn0 * (gx0 < -0.01 ? 1 : -1);
    gy0 += zn0 * (gy0 < -0.01 ? 1 : -1);

    float4 gx1 = lerp(-1, 1, frac(floor(ixy1 / 7) / 7));
    float4 gy1 = lerp(-1, 1, frac(floor(ixy1 % 7) / 7));
    float4 gz1 = 1 - abs(gx1) - abs(gy1);

    bool4 zn1 = gz1 < -0.01;
    gx1 += zn1 * (gx1 < -0.01 ? 1 : -1);
    gy1 += zn1 * (gy1 < -0.01 ? 1 : -1);

    float3 g000 = normalize(float3(gx0.x, gy0.x, gz0.x));
    float3 g100 = normalize(float3(gx0.y, gy0.y, gz0.y));
    float3 g010 = normalize(float3(gx0.z, gy0.z, gz0.z));
    float3 g110 = normalize(float3(gx0.w, gy0.w, gz0.w));
    float3 g001 = normalize(float3(gx1.x, gy1.x, gz1.x));
    float3 g101 = normalize(float3(gx1.y, gy1.y, gz1.y));
    float3 g011 = normalize(float3(gx1.z, gy1.z, gz1.z));
    float3 g111 = normalize(float3(gx1.w, gy1.w, gz1.w));

    float n000 = dot(g000, pf0);
    float n100 = dot(g100, float3(pf1.x, pf0.y, pf0.z));
    float n010 = dot(g010, float3(pf0.x, pf1.y, pf0.z));
    float n110 = dot(g110, float3(pf1.x, pf1.y, pf0.z));
    float n001 = dot(g001, float3(pf0.x, pf0.y, pf1.z));
    float n101 = dot(g101, float3(pf1.x, pf0.y, pf1.z));
    float n011 = dot(g011, float3(pf0.x, pf1.y, pf1.z));
    float n111 = dot(g111, pf1);

    float3 fade_xyz = wglnoise_fade(pf0);
    float4 n_z = lerp(float4(n000, n100, n010, n110),
                      float4(n001, n101, n011, n111), fade_xyz.z);
    float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
    float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x);
    return 1.46 * n_xyz;
}

// Classic Perlin noise
float ClassicNoise(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    return ClassicNoise_impl(i, f, i + 1, f - 1);
}

// Classic Perlin noise, periodic variant
float PeriodicNoise(float3 p, float3 rep)
{
    float3 i0 = wglnoise_mod(floor(p), rep);
    float3 i1 = wglnoise_mod(i0 + 1, rep);
    float3 f = frac(p);
    return ClassicNoise_impl(i0, f, i1, f - 1);
}

float3 SimplexNoiseGrad(float2 v)
{
    const float C1 = (3 - sqrt(3)) / 6;
    const float C2 = (sqrt(3) - 1) / 2;

    // First corner
    float2 i  = floor(v + dot(v, C2));
    float2 x0 = v -   i + dot(i, C1);

    // Other corners
    float2 i1 = x0.x > x0.y ? float2(1, 0) : float2(0, 1);
    float2 x1 = x0 + C1 - i1;
    float2 x2 = x0 + C1 * 2 - 1;

    // Permutations
    i = wglnoise_mod289(i); // Avoid truncation effects in permutation
    float3 p = wglnoise_permute(    i.y + float3(0, i1.y, 1));
    p = wglnoise_permute(p + i.x + float3(0, i1.x, 1));

    // Gradients: 41 points uniformly over a unit circle.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    float3 phi = p / 41 * 3.14159265359 * 2;
    float2 g0 = float2(cos(phi.x), sin(phi.x));
    float2 g1 = float2(cos(phi.y), sin(phi.y));
    float2 g2 = float2(cos(phi.z), sin(phi.z));

    // Compute noise and gradient at P
    float3 m  = float3(dot(x0, x0), dot(x1, x1), dot(x2, x2));
    float3 px = float3(dot(g0, x0), dot(g1, x1), dot(g2, x2));

    m = max(0.5 - m, 0);
    float3 m3 = m * m * m;
    float3 m4 = m * m3;

    float3 temp = -8 * m3 * px;
    float2 grad = m4.x * g0 + temp.x * x0 +
                  m4.y * g1 + temp.y * x1 +
                  m4.z * g2 + temp.z * x2;

    return 99.2 * float3(grad, dot(m4, px));
}

float SimplexNoise(float2 v)
{
    return SimplexNoiseGrad(v).z;
}

float4 SimplexNoiseGrad(float3 v)
{
    // First corner
    float3 i  = floor(v + dot(v, 1.0 / 3));
    float3 x0 = v   - i + dot(i, 1.0 / 6);

    // Other corners
    float3 g = x0.yzx <= x0.xyz;
    float3 l = 1 - g;
    float3 i1 = min(g.xyz, l.zxy);
    float3 i2 = max(g.xyz, l.zxy);

    float3 x1 = x0 - i1 + 1.0 / 6;
    float3 x2 = x0 - i2 + 1.0 / 3;
    float3 x3 = x0 - 0.5;

    // Permutations
    i = wglnoise_mod289(i); // Avoid truncation effects in permutation
    float4 p = wglnoise_permute(    i.z + float4(0, i1.z, i2.z, 1));
    p = wglnoise_permute(p + i.y + float4(0, i1.y, i2.y, 1));
    p = wglnoise_permute(p + i.x + float4(0, i1.x, i2.x, 1));

    // Gradients: 7x7 points over a square, mapped onto an octahedron.
    // The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
    float4 gx = lerp(-1, 1, frac(floor(p / 7) / 7));
    float4 gy = lerp(-1, 1, frac(floor(p % 7) / 7));
    float4 gz = 1 - abs(gx) - abs(gy);

    bool4 zn = gz < -0.01;
    gx += zn * (gx < -0.01 ? 1 : -1);
    gy += zn * (gy < -0.01 ? 1 : -1);

    float3 g0 = normalize(float3(gx.x, gy.x, gz.x));
    float3 g1 = normalize(float3(gx.y, gy.y, gz.y));
    float3 g2 = normalize(float3(gx.z, gy.z, gz.z));
    float3 g3 = normalize(float3(gx.w, gy.w, gz.w));

    // Compute noise and gradient at P
    float4 m  = float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3));
    float4 px = float4(dot(g0, x0), dot(g1, x1), dot(g2, x2), dot(g3, x3));

    m = max(0.5 - m, 0);
    float4 m3 = m * m * m;
    float4 m4 = m * m3;

    float4 temp = -8 * m3 * px;
    float3 grad = m4.x * g0 + temp.x * x0 +
                  m4.y * g1 + temp.y * x1 +
                  m4.z * g2 + temp.z * x2 +
                  m4.w * g3 + temp.w * x3;

    return 107 * float4(grad, dot(m4, px));
}

float SimplexNoise(float3 v)
{
    return SimplexNoiseGrad(v).w;
}

#endif