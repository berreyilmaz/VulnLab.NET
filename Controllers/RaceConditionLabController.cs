using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace VulnLab.NET.Controllers;

[Route("labs/race-condition")]
public class RaceConditionLabController : Controller
{
    private const int InitialBalance = 1000;
    private static readonly object BalanceLock = new();

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/RaceCondition.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(int requestCount, int amount)
    {
        ViewData["Mode"] = "Vulnerable";
        var summary = RunSimulation(requestCount, amount, secureMode: false);
        ApplySummary(summary);
        return View("~/Views/Labs/RaceCondition.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(int requestCount, int amount)
    {
        ViewData["Mode"] = "Secure";
        var summary = RunSimulation(requestCount, amount, secureMode: true);
        ApplySummary(summary);
        return View("~/Views/Labs/RaceCondition.cshtml");
    }

    private static SimulationSummary RunSimulation(int requestCount, int amount, bool secureMode)
    {
        requestCount = Math.Clamp(requestCount, 1, 500);
        amount = Math.Clamp(amount, 1, 200);

        var balance = InitialBalance;
        var successfulWithdrawals = 0;
        var blockedWithdrawals = 0;
        var trace = new ConcurrentQueue<string>();

        Parallel.For(0, requestCount, i =>
        {
            if (secureMode)
            {
                lock (BalanceLock)
                {
                    if (balance >= amount)
                    {
                        balance -= amount;
                        successfulWithdrawals++;
                        trace.Enqueue($"Req#{i + 1}: basarili ({amount})");
                    }
                    else
                    {
                        blockedWithdrawals++;
                        trace.Enqueue($"Req#{i + 1}: yetersiz bakiye");
                    }
                }
            }
            else
            {
                // Intentional race: check and update are not atomic.
                if (balance >= amount)
                {
                    var snapshot = balance;
                    Thread.Sleep(1);
                    balance = snapshot - amount;
                    successfulWithdrawals++;
                    trace.Enqueue($"Req#{i + 1}: basarili ({amount})");
                }
                else
                {
                    blockedWithdrawals++;
                    trace.Enqueue($"Req#{i + 1}: yetersiz bakiye");
                }
            }
        });

        var expectedMinBalance = InitialBalance - (successfulWithdrawals * amount);
        var raceDetected = !secureMode && balance != expectedMinBalance;

        return new SimulationSummary(
            requestCount,
            amount,
            successfulWithdrawals,
            blockedWithdrawals,
            InitialBalance,
            balance,
            expectedMinBalance,
            raceDetected,
            trace.Take(14).ToArray());
    }

    private void ApplySummary(SimulationSummary summary)
    {
        ViewData["RequestCount"] = summary.RequestCount;
        ViewData["Amount"] = summary.Amount;
        ViewData["InitialBalance"] = summary.InitialBalance;
        ViewData["FinalBalance"] = summary.FinalBalance;
        ViewData["ExpectedBalance"] = summary.ExpectedBalance;
        ViewData["SuccessCount"] = summary.SuccessfulWithdrawals;
        ViewData["BlockedCount"] = summary.BlockedWithdrawals;
        ViewData["RaceDetected"] = summary.RaceDetected;
        ViewData["TraceLines"] = summary.TraceLines;
    }

    private sealed record SimulationSummary(
        int RequestCount,
        int Amount,
        int SuccessfulWithdrawals,
        int BlockedWithdrawals,
        int InitialBalance,
        int FinalBalance,
        int ExpectedBalance,
        bool RaceDetected,
        string[] TraceLines);
}
