using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProtobuf.UIElements
{
    public class FileSystemUtility
    {
        public static string GetUniquePath(string rootPath, string name)
        {
            var path = Path.Combine(rootPath, name);
            var hasExt = Path.HasExtension(name);
            var fileName = hasExt ? Path.GetFileNameWithoutExtension(name) : name;
            var digits = fileName.Reverse().TakeWhile(c => char.IsDigit(c));
            var number = new string(digits.Reverse().ToArray());
            int i = 1;
            if (int.TryParse(number, out int n))
            {
                i = n;
            }
            fileName = fileName.Substring(0, fileName.Length - digits.Count()).TrimEnd();

            if (hasExt)
            {
                var fileExt = Path.GetExtension(name);
                while (File.Exists(path))
                {
                    path = string.Format("{0} {1}{2}", Path.Combine(rootPath, fileName), i++, fileExt);
                }
            }
            else
            {
                var oriPath = path;
                while (Directory.Exists(path))
                {
                    path = string.Format("{0} {1}", oriPath, i++);
                }
            }
            return path;
        }

        public static bool TryGetNameEndNumber(string name,out int number)
        {
            if (Path.HasExtension(name)) name = Path.GetFileNameWithoutExtension(name);
            var digits = name.Reverse().TakeWhile(c => char.IsDigit(c));
            var numStr = new string(digits.Reverse().ToArray());
            return int.TryParse(numStr, out number);
        }
    }
}