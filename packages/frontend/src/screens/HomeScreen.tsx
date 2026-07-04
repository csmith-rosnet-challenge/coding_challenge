import { useState } from 'react';
import StatusForm from '../components/StatusForm';
import ResultTable from '../components/ResultTable';
import HistoryTable from '../components/HistoryTable';
import DummyComponent from '../components/DummyComponent';
import HistoryCharts from '../components/HistoryCharts';
import {
  fetchExternalStatus,
  fetchExternalStatusHistory,
} from '../services/statusService';
import type {
  ExternalStatusResult,
  ExternalStatusHistoryItem,
} from '../services/types';

export default function HomeScreen() {
  const [urlsText, setUrlsText] = useState('https://www.google.com\nhttps://www.github.com');
  const [timeoutMs, setTimeoutMs] = useState<number>(5000);
  const [maxConcurrency, setMaxConcurrency] = useState<number>(5);
  const [results, setResults] = useState<ExternalStatusResult[]>([]);
  const [history, setHistory] = useState<ExternalStatusHistoryItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadStatus = async () => {
    setError(null);
    setLoading(true);

    try {
      const data = await fetchExternalStatus({ urlsText, timeoutMs, maxConcurrency });
      setResults(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const loadHistory = async () => {
    setHistoryLoading(true);
    setError(null);

    try {
      const data = await fetchExternalStatusHistory(50);
      setHistory(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to load history');
    } finally {
      setHistoryLoading(false);
    }
  };

  return (
    <div className="app-shell">
      <header>
        <h1>External Status Dashboard</h1>
        <p>Fetch live status data or view historical results from the backend.</p>
      </header>

      <DummyComponent />

      <StatusForm
        urlsText={urlsText}
        timeoutMs={timeoutMs}
        maxConcurrency={maxConcurrency}
        onUrlsChange={setUrlsText}
        onTimeoutChange={setTimeoutMs}
        onConcurrencyChange={setMaxConcurrency}
        onRunStatus={loadStatus}
        onLoadHistory={loadHistory}
        loading={loading}
        historyLoading={historyLoading}
        error={error}
      />

      <section className="card">
        <h2>Latest results</h2>
        <ResultTable results={results} />
      </section>

      <section className="card">
        <h2>History</h2>
        <HistoryCharts history={history} />
        <hr />
      </section>
    </div>
  );
}
