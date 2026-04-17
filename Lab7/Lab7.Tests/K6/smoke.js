import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from './config.js';

export const options = {
  vus: 1,
  duration: '1m',
};

export default function () {
  const res = http.get(`${BASE_URL}/api/students`);

  check(res, {
    'status is 200': (r) => r.status === 200,
  });

  sleep(1);
}