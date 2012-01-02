using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class RtsdMapIndex
    {
        /* 5541-I   102 45 E - 103 00 E / 16 30 N - 16 15 N
         * 5541-II  102 45 E - 103 00 E / 16 15 N - 16 00 N
         * 5541-III 102 30 E - 102 45 E / 16 15 N - 16 00 N
         * 5541-IV  102 30 E - 102 45 E / 16 30 N - 16 15 N
         * 
         * 5441-I   102 15 E - 102 30 E / 16 30 N - 16 15 N
         * 5542-I   102 45 E - 103 00 E / 17 00 N - 16 45 N
         * 
         * starting 4833/4933 to south longitude offset different
         * 
         * northwest corner: 4449
         * northeast corner: 6149
         */

        private static List<Int32> ValidFourSquares = new List<Int32>() 
        {
                                    4849, 4949, 5049, 5149,
                  4548, 4648, 4748, 4848, 4948, 5048, 5148, 5248,
                  4547, 4647, 4747, 4847, 4947, 5047, 5147, 5247,
            4446, 4546, 4646, 4746, 4846, 4946, 5046, 5146, 5246,
            4445, 4545, 4645, 4745, 4845, 4945, 5045, 5145, 5245, 5345, 5445, 5545, 5645, 5745, 5845,
                  4544, 4644, 4744, 4844, 4944, 5044, 5144, 5244, 5344, 5444, 5544, 5644, 5744, 5844, 5944,
                        4643, 4743, 4843, 4943, 5043, 5143, 5243, 5343, 5443, 5543, 5643, 5743, 5843, 5943,
                        4642, 4742, 4842, 4942, 5042, 5142, 5242, 5342, 5442, 5542, 5642, 5742, 5842, 5942,
                              4741, 4841, 4941, 5041, 5141, 5241, 5341, 5441, 5541, 5641, 5741, 5841, 5941, 6041,
                              4740, 4840, 4940, 5040, 5140, 5240, 5340, 5440, 5540, 5640, 5740, 5840, 5940, 6040, 6140, 
                        4639, 4739, 4839, 4939, 5039, 5139, 5239, 5339, 5439, 5539, 5639, 5739, 5839, 5939, 6039, 6139,
                        4638, 4738, 4838, 4938, 5038, 5138, 5238, 5338, 5438, 5538, 5638, 5738, 5838, 5938, 6038, 6138,
                              4737, 4837, 4937, 5037, 5137, 5237, 5337, 5437, 5537, 5637, 5737, 5837, 5937, 6037,
                                    4836, 4936, 5036, 5136, 5236, 5336, 5436, 5536,
                                    4835, 4935, 5035, 5135, 5235, 5335, 5435, 5535,
                                    4834, 4934, 5034, 5134, 5234, 5334, 5434, 5534,
                                                                        5433, 5533,
                                                                        5432, 5532,

                        4627, 4727, 4827, 4927,
                        4626, 4726, 4826, 4926, 5026,
                        4625, 4725, 4825, 4925, 5025,
                        4624, 4724, 4824, 4924, 5024,
                                    4823, 4923, 5023, 5123,
                                          4922, 5022, 5122, 5222, 5322,
                                                5021, 5121, 5221, 5321, 5421,
                                                            5220, 5320, 5420, 
        };

        // special cases: 4822 II - different offset in latitude and longitude
        // special cases: 4827 IV, 4927 IV, 4927 I - 3 minute more wide in longitude
        // 4627 I, 4727 IV, 4727 I, 4827 IV - starts 3 minute more east; 4827 IV 4 minute more wide
        // 4928 II 6 minute more east
        // 4928 II, 4928 III 18.5 minute more high

        public static String IndexL7018(GeoPoint location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            Int32 longitude = (Int32)Math.Truncate(location.Longitude * 4.0);
            Int32 latitude = (Int32)Math.Truncate(location.Latitude * 4.0);

            Int32 latitudeIndex = (latitude / 2) - 33 + 42;
            Int32 longitudeIndex = (longitude / 2) - 205 + 55;

            Int32 index = longitudeIndex * 100 + latitudeIndex;

            // ToDo: Collect all the rectangles not aligned to 15'
            // ToDo: Collect all rectangles allowed, to return ArgumentException when out of area

            Int32 segment = ((longitude % 2) * 2 + (latitude % 2));
            String segmentName = String.Empty;
            switch (segment)
            {
                case 0: 
                    segmentName = "III";
                    break;
                case 1:
                    segmentName = "IV";
                    break;
                case 2:
                    segmentName = "II";
                    break;
                case 3:
                    segmentName = "I";
                    break;
            }

            String result = String.Format("{0:####} {1}", index, segmentName);
            if (!ValidFourSquares.Contains(index))
            {
                result = "invalid";
            }

            return result;
        }
    }
}
