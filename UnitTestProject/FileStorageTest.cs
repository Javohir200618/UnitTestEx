using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Reflection;
using UnitTestEx;
using Assert = NUnit.Framework.Assert;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestClass]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int NEW_SIZE = 5;

        public FileStorage storage = new FileStorage(NEW_SIZE);

        /* ПРОВАЙДЕРЫ */

        static object[] NewFilesData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
            new object[] { new File(SPACE_STRING, WRONG_SIZE_CONTENT_STRING) },
            new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
        };

        static object[] FilesForDeleteData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
            new object[] { null, TIC_TOC_TOE_STRING }
        };

        static object[] NewExceptionFileData = {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) }
        };

        /* Тестирование записи файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest(File file)
        {
            //удаляем файлы , если в памяти есть хотя бы 1 файл
            if (storage.IsExists(file.GetFilename()))
                storage.DeleteAllFiles();
            try
            {
                //после записи файла, его нельзя еще раз записать
                if (storage.Write(file))
                    Assert.False(storage.Write(file));
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
            }
            storage.DeleteAllFiles();
        }

        /* Тестирование записи дублирующегося файла */
        [Test, TestCaseSource(nameof(NewExceptionFileData))]
        public void WriteExceptionTest(File file) {
            bool isException = false;
            try
            {
                storage.Write(file);
                Assert.False(storage.Write(file));
                storage.DeleteAllFiles();
            } 
            catch (FileNameAlreadyExistsException)
            {
                isException = true;
            }
            Assert.True(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void IsExistsTest(File file)
        {
            //если файл существует удаляем его
            if (storage.IsExists(file.GetFilename()))
                storage.DeleteAllFiles();
            try
            {
                //если файл записался, значит он существует
                if (storage.Write(file))
                    Assert.True(storage.IsExists(file.GetFilename()));
            }
            catch (FileNameAlreadyExistsException e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
                Assert.False(storage.IsExists(file.GetFilename()));
            }
            storage.DeleteAllFiles();
        }

        /* Тестирование удаления файла */
        [Test, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest(File file, String fileName)
        {
            //записываем файл, а потом проверяем возможность удалить его
            try
            {
                storage.Write(file);
                // Существуюший файл можно удалить
                if (storage.IsExists(fileName))
                    Assert.True(storage.Delete(fileName));
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
                Assert.False(storage.Delete(fileName));
            }
        }

        /* Тестирование получения файлов */
        [Test]
        public void GetFilesTest()
        {
            foreach (File el in storage.GetFiles()) 
            {
                Assert.NotNull(el);
            }
        }

        // Почти эталонный
        /* Тестирование получения файла */
        public void GetFileTest(File expectedFile)
        {
            File actualfile;
            // файл не существует
            if (!storage.IsExists(expectedFile.GetFilename()))
            {
                // файл записывается
                if (storage.Write(expectedFile))
                {
                    // берём файл
                    actualfile = storage.GetFile(expectedFile.GetFilename());
                    Assert.NotNull(actualfile);
                }
            }
            else
            {
                // берём файл
                actualfile = storage.GetFile(expectedFile.GetFilename());
                Assert.NotNull(actualfile);
            }
        }
        /* Тестирование получения файла с неверным именем */
        [Test]
        public void GetFileWhenFilenameIsIncorrect()
        {
            string incorrectFilename = "INVALIDFILENAME";

            File actualFile = storage.GetFile(incorrectFilename);
            Assert.Null(actualFile);
        }
        /* Тестирование удаления файла, который уже был удален */
        [Test]
        public void DeleteWhenFileAlreadyDeleted()
        {
            File file = new File("EXISTINGFILE", "CONTENTSTRING");
            storage.Delete(file.GetFilename()); // Сначала удаляем файл

            Assert.False(storage.Delete(file.GetFilename()));
        }
        /* Тестирование получения файла из пустого хранилища */
        [Test]
        public void GetFileFromEmptyStorage()
        {
            string filename = "NONEXISTENTFILE";

            File actualFile = storage.GetFile(filename);
            Assert.Null(actualFile);
        }

    }
}
