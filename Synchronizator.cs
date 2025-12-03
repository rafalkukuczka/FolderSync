using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync
{
    internal class Synchronizator
    {
#pragma warning disable IDE0305
#pragma warning disable IDE0042

        public static (HashSet<string> dirs, HashSet<(string file, string tick)> files) GetSnapshot(string dir)
        {
            Console.WriteLine("Scaning directory: " + dir);

            string[] directories = GetDirectories(dir);

            var dirHash = directories.Select(GetRelativePath).ToHashSet();

            var fileHash = directories.SelectMany(Directory.EnumerateFiles).AsParallel()
               .Select(file => (name: GetRelativePath(file), ticks: GetHash(file)))
               .ToHashSet();

            return (dirHash, fileHash);

            static string[] GetDirectories(string dir)
            {
                var directories = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
                directories = directories.Append(dir).ToArray();
                return directories;
            }

            string GetHash(string path) => GetMd5(path);

            string GetMd5(string path)
            {
                using var md5 = System.Security.Cryptography.MD5.Create();
                using var stream = File.OpenRead(path);
                var hash = md5.ComputeHash(stream);
                return Convert.ToHexString(hash);
            }

            string GetRelativePath(string path) => Path.GetRelativePath(dir, path);
        }

        public static void Synchronize(string sourcePath, string dstPath)
        {
            var snapshotSrc = GetSnapshot(sourcePath);
            var snapshotDst = GetSnapshot(dstPath);

            var filesToRemove = snapshotDst.files.Where(x => !snapshotSrc.files.Contains(x));

            if (filesToRemove.Count() == 0)
                Console.WriteLine("Deleting files(s)...skipping");
            else
                Console.WriteLine("Deleting file(s): " + filesToRemove.Count());

            foreach (var fileToRemove in filesToRemove)
            {
                Console.WriteLine("Deleting file: " + Path.Combine(dstPath, fileToRemove.file));
                File.Delete(Path.Combine(dstPath, fileToRemove.file));
            }

            var dirsToRemove = snapshotDst.dirs.Where(x => !snapshotSrc.dirs.Contains(x)).OrderByDescending(d => d.Length);

            if (dirsToRemove.Count() == 0)
                Console.WriteLine("Deleting dir(s)...skipping");
            else
                Console.WriteLine("Deleting dir(s): " + dirsToRemove.Count());

            foreach (var dirToRemove in dirsToRemove)
            {
                Console.WriteLine("Deleting dir: " + Path.Combine(dstPath, dirToRemove));
                Directory.Delete(Path.Combine(dstPath, dirToRemove), true);
            }

            var dirsToAdd = snapshotSrc.dirs.Where(x => !snapshotDst.dirs.Contains(x));

            if (dirsToAdd.Count() == 0)
                Console.WriteLine("Adding dir(s)...skipping");
            else
                Console.WriteLine("Adding dir(s): " + dirsToAdd.Count());

            foreach (var dirToAdd in dirsToAdd)
            {
                Console.WriteLine("Adding dir: " + Path.Combine(dstPath, dirToAdd));
                Directory.CreateDirectory(Path.Combine(dstPath, dirToAdd));
            }

            var filesToAdd = snapshotSrc.files.Where(x => !snapshotDst.files.Contains(x));

            if (filesToAdd.Count() == 0)
                Console.WriteLine("Adding file(s)...skipping");
            else
                Console.WriteLine("Adding file(s): " + filesToAdd.Count());

            foreach (var fileToAdd in filesToAdd)
            {
                Console.WriteLine("Adding file: " + Path.Combine(dstPath, fileToAdd.file));
                File.Copy(Path.Combine(sourcePath, fileToAdd.file), Path.Combine(dstPath, fileToAdd.file), true);
            }

        }
    }

#pragma warning restore IDE0305
#pragma warning restore IDE0042
}
