﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RelaxBarWeb_MVC.Utils
{
    public static class DecimalUtil
    {
        /// <summary>
        /// 将小数值按指定的小数位数截断
        /// </summary>
        /// <param name="d">要截断的小数</param>
        /// <param name="s">小数位数，s大于等于0，小于等于28</param>
        /// <returns></returns>
        public static decimal ToFixed(this decimal d, int s)
        {
            decimal sp = Convert.ToDecimal(Math.Pow(10, s));

            if (d < 0)
                return Math.Truncate(d) + Math.Ceiling((d - Math.Truncate(d)) * sp) / sp;
            else
                return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;
        }
    }
}