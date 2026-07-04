import type { ChangeEvent } from 'react';

interface StatusFormProps {
  urlsText: string;
  timeoutMs: number;
  maxConcurrency: number;
  onUrlsChange: (value: string) => void;
  onTimeoutChange: (value: number) => void;
  onConcurrencyChange: (value: number) => void;
  onRunStatus: () => void | Promise<void>;
  onLoadHistory: () => void | Promise<void>;
  loading: boolean;
  historyLoading: boolean;
  error: string | null;
}

export default function StatusForm({
  urlsText,
  timeoutMs,
  maxConcurrency,
  onUrlsChange,
  onTimeoutChange,
  onConcurrencyChange,
  onRunStatus,
  onLoadHistory,
  loading,
  historyLoading,
  error,
}: StatusFormProps) {
  return (
    <section className="card">
      <h2>On-demand status check</h2>
      <label>
        URLs (one per line):
        <textarea
          value={urlsText}
          onChange={(e: ChangeEvent<HTMLTextAreaElement>) => onUrlsChange(e.target.value)}
          rows={5}
        />
      </label>

      <div className="controls-row">
        <label>
          Timeout (ms):
          <input
            type="number"
            min="100"
            value={timeoutMs}
            onChange={(e: ChangeEvent<HTMLInputElement>) => onTimeoutChange(Number(e.target.value))}
          />
        </label>
        <label>
          Max concurrency:
          <input
            type="number"
            min="1"
            value={maxConcurrency}
            onChange={(e: ChangeEvent<HTMLInputElement>) => onConcurrencyChange(Number(e.target.value))}
          />
        </label>
      </div>

      <div className="button-row">
        <button onClick={onRunStatus} disabled={loading}>
          {loading ? 'Checking...' : 'Run status check'}
        </button>
        <button onClick={onLoadHistory} disabled={historyLoading}>
          {historyLoading ? 'Loading history...' : 'Load history'}
        </button>
      </div>

      {error && <div className="error">{error}</div>}
    </section>
  );
}
