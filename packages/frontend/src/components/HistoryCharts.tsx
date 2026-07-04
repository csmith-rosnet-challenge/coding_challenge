import React, { useState } from 'react';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import type { ExternalStatusHistoryItem } from '../services/types';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

interface Props {
  history: ExternalStatusHistoryItem[];
}

const palette = [
  '#3366CC',
  '#DC3912',
  '#FF9900',
  '#109618',
  '#990099',
  '#3B3EAC',
  '#0099C6',
  '#DD4477',
];

function formatTimeLabel(iso: string) {
  const d = new Date(iso);
  return d.toLocaleString();
}

export default function HistoryCharts({ history }: Props) {
  if (!history || history.length === 0) {
    return <p>No history available to chart.</p>;
  }

  // Collect unique sorted timestamps
  const timestamps = Array.from(new Set(history.map((h) => h.checkedAt))).sort((a, b) => new Date(a).getTime() - new Date(b).getTime());

  // Group by URL
  const byUrl = history.reduce<Record<string, ExternalStatusHistoryItem[]>>((acc, item) => {
    (acc[item.url] ||= []).push(item);
    return acc;
  }, {});

  const urls = Object.keys(byUrl).sort();

  const labels = timestamps.map((t) => formatTimeLabel(t));

  const [visibleUrl, setVisibleUrl] = useState<string | null>(null);

  const datasets = urls.map((url, i) => {
    const map = new Map(byUrl[url].map((it) => [it.checkedAt, it.durationMs]));
    const data = timestamps.map((ts) => (map.has(ts) ? map.get(ts) as number : null));
    const hidden = visibleUrl ? url !== visibleUrl : false;
    return {
      label: url,
      data,
      borderColor: palette[i % palette.length],
      backgroundColor: palette[i % palette.length],
      tension: 0.2,
      spanGaps: true,
      showLine: true,
      pointRadius: 3,
      hidden,
    };
  });

  const data = { labels, datasets };

  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top' as const,
        // clicking a legend label filters to that URL (click again to reset)
        onClick: function (_e: MouseEvent, legendItem: any) {
          const url = String(legendItem.text || '');
          setVisibleUrl((prev) => (prev === url ? null : url));
        },
      },
      title: { display: true, text: 'Response time over time (ms) — all URLs' },
    },
    scales: {
      y: { title: { display: true, text: 'Response time (ms)' }, beginAtZero: true },
      x: {
        title: { display: true, text: 'Checked At' },
        ticks: {
          // show label only on every 5th tick and ensure we use the formatted label
          callback: function (value: unknown, index: number) {
            return index % 5 === 0 ? labels[index] ?? String(value) : '';
          },
          // allow diagonal labels for readability
          maxRotation: 45,
          minRotation: 30,
          autoSkip: false,
        },
      },
    },
  };

  // Find non-200 entries
  const errors = history.filter((h) => h.statusCode !== 200 && h.statusCode !== null && h.statusCode !== undefined);
  const visibleErrors = visibleUrl ? errors.filter((h) => h.url === visibleUrl) : errors;

  return (
    <div className="history-charts">
      <div style={{ marginBottom: 24 }}>
        <Line data={data} options={options} />
      </div>

      <section>
        <h3>Non-200 responses</h3>
        {visibleErrors.length === 0 ? (
          <p>All recent checks returned 200 status codes.</p>
        ) : (
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
              {visibleErrors
                .slice()
                .sort((a, b) => new Date(b.checkedAt).getTime() - new Date(a.checkedAt).getTime())
                .map((item) => (
                  <tr key={`${item.url}-${item.checkedAt}-${item.id}`}>
                    <td>{item.url}</td>
                    <td>{item.statusCode}</td>
                    <td>{item.durationMs} ms</td>
                    <td>{item.error || '—'}</td>
                    <td>{new Date(item.checkedAt).toLocaleString()}</td>
                  </tr>
                ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}

