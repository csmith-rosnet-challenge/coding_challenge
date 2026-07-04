import type { ExternalStatusResult } from '../services/types';

interface ResultTableProps {
  results: ExternalStatusResult[];
}

export default function ResultTable({ results }: ResultTableProps) {
  if (results.length === 0) {
    return <p>No live results yet. Run a status check.</p>;
  }

  return (
    <table>
      <thead>
        <tr>
          <th>URL</th>
          <th>Status</th>
          <th>Duration</th>
          <th>Error</th>
        </tr>
      </thead>
      <tbody>
        {results.map((item) => (
          <tr key={item.url}>
            <td>{item.url}</td>
            <td>{item.statusCode ?? 'N/A'}</td>
            <td>{item.durationMs} ms</td>
            <td>{item.error || '—'}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
