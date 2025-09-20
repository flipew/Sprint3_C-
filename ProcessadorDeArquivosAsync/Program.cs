using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//TELA INICIAL E LEITURA 

Console.WriteLine("--- Processador Assíncrono de Arquivos de Texto ---");
Console.WriteLine();

string? diretorioPath;

while (true)
{
    Console.Write("Por favor, coloca o caminho do diretório .txt ai lerdão: ");
    diretorioPath = Console.ReadLine();

    if (!string.IsNullOrEmpty(diretorioPath) && Directory.Exists(diretorioPath))
    {
        break;
    }

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Esse caminho ta inválido. Tente novamente.");
    Console.ResetColor();
}

var arquivos = Directory.GetFiles(diretorioPath, "*.txt");

if (!arquivos.Any())
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Nenhum arquivo .txt encontrado no diretório.");
    Console.ResetColor();
    Console.WriteLine("Pressione qualquer tecla para sair.");
    Console.ReadKey();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n{arquivos.Length} arquivo(s) .txt encontrado(s).");
Console.ResetColor();

Console.WriteLine("Pressione qualquer tecla para iniciar o processamento :)");
Console.ReadKey();
Console.WriteLine();

//PROCESSAMENTO ASSÍNCRONO 

var stopwatch = Stopwatch.StartNew();

var tasks = new List<Task<string>>();
foreach (var arquivoPath in arquivos)
{
    tasks.Add(ProcessarArquivoAsync(arquivoPath));
}

string[] resultados = await Task.WhenAll(tasks);

stopwatch.Stop(); //cronômetro

//GERAÇÃO DO RELATÓRIO

string exportPath = Path.Combine(AppContext.BaseDirectory, "export");
Directory.CreateDirectory(exportPath); // Cria a pasta "export" se ela não existir.
string relatorioPath = Path.Combine(exportPath, "relatorio.txt");

// Resultados do arquivo
await File.WriteAllLinesAsync(relatorioPath, resultados);


Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\nProcessamento finalizado!");
Console.WriteLine($"Tempo total de execução: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
Console.WriteLine($"Relatório consolidado foi salvo em: {relatorioPath}");
Console.ResetColor();

Console.WriteLine("\nPressione qualquer tecla para sair.");
Console.ReadKey();


//MÉTODO AUXILIAR PARA PROCESSAR CADA ARQUIVO

static async Task<string> ProcessarArquivoAsync(string arquivoPath)
{
    string nomeArquivo = Path.GetFileName(arquivoPath);
    Console.WriteLine($"Processando arquivo: {nomeArquivo}...");

    // Lê todas as linhas do arquivo de forma assíncrona.
    string[] linhas = await File.ReadAllLinesAsync(arquivoPath);
    int totalLinhas = linhas.Length;
    int totalPalavras = 0;

    foreach (var linha in linhas)
    {
        // Separação
        string[] palavras = linha.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        totalPalavras += palavras.Length;
    }

    // Retorno para relatório
    return $"{nomeArquivo} - {totalLinhas} linhas - {totalPalavras} palavras";
}

