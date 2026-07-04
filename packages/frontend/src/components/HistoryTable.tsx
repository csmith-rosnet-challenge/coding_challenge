import type { ExternalStatusHistoryItem } from '../services/types';

interface HistoryTableProps {
  history: ExternalStatusHistoryItem[];
}

export default function HistoryTable({ history }: HistoryTableProps) {
  if (history.length === 0) {
    return <p>Load history to see persisted checks from the background fetcher.</p>;
  }

  return (
    <table>
      <thead>
        <tr>
          <th>URL</th>
          <th>Status</th>
          <th>Duration</th>
          <th>Error</th>
          <th>Checked At</th>
        </tr>
      </thead>
      <tbody>
        {history.map((item) => (
          <tr key={`${item.url}-${item.checkedAt}-${item.id}`}>
            <td>{item.url}</td>
            <td>{item.statusCode ?? 'N/A'}</td>
            <td>{item.durationMs} ms</td>
            <td>{item.error || '—'}</td>
            <td>{new Date(item.checkedAt).toLocaleString()}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
