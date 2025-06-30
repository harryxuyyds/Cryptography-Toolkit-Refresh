using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography_Toolkit.Helpers
{
    public class EllipticCurveHelper
    {
        private readonly Common _common;

        public EllipticCurveHelper()
        {
            _common = new Common();
        }

        public int PointSlopeCalc(int pointGx, int pointGy, int pointPx, int pointPy, int a, int p)
        {
            int pointSlope = -1;
            if (pointGx % p != pointPx % p)
            {
                int temp = pointPx - pointGx;
                while (temp < 0)
                    temp = (temp + p) % p;
                (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(p, temp);
                pointSlope = (pointPy - pointGy) * t % p;
                while (pointSlope < 0)
                    pointSlope = (pointSlope + p) % p;
            }
            else if ((pointGx % p == pointPx % p) & (pointGy % p == pointPy % p))
            {
                (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(p, 2 * pointGy);
                pointSlope = (3 * pointGx * pointGx + a) * t % p;
                while (pointSlope < 0)
                    pointSlope = (pointSlope + p) % p;
            }
            return pointSlope;
        }

        public int[] PointDoubleCalc(int pointGx, int pointGy, int a, int p)
        {
            int pointSlope = PointSlopeCalc(pointGx, pointGy, pointGx, pointGy, a, p);
            int newPointX = (pointSlope * pointSlope - 2 * pointGx) % p;
            int newPointY = (pointSlope * (pointGx - newPointX) - pointGy) % p;
            while (newPointX < 0)
                newPointX = (newPointX + p) % p;
            while (newPointY < 0)
                newPointY = (newPointY + p) % p;
            int[] newPointLocate = [newPointX, newPointY];
            return newPointLocate;
        }

        public int[] PointAddCalc(int pointGx, int pointGy, int pointPx, int pointPy, int a, int p)
        {
            int pointSlope = PointSlopeCalc(pointGx, pointGy, pointPx, pointPy, a, p);
            if (pointSlope != -1)
            {
                int newPointX = (pointSlope * pointSlope - pointGx - pointPx) % p;
                int newPointY = (pointSlope * (pointGx - newPointX) - pointGy) % p;
                while (newPointX < 0)
                    newPointX = (newPointX + p) % p;
                while (newPointY < 0)
                    newPointY = (newPointY + p) % p;
                int[] newPointLocate = [newPointX, newPointY];
                return newPointLocate;
            }
            else
            {
                int[] newPointLocate = [0, 0];
                return newPointLocate;
            }
        }
    }
}
