import http from 'k6/http';
import { sleep } from 'k6';
import { BASE_URL } from './config.js';

export const options = {
  stages: [
    { duration: '2m', target: 10 },
    { duration: '2m', target: 50 },
    { duration: '2m', target: 100 },
    { duration: '2m', target: 250 },
    { duration: '2m', target: 500 },
  ],
};

export default function () {
  http.get(`${BASE_URL}/api/students`);
  sleep(1);
}