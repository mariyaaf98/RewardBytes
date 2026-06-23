import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UploadService {

  private readonly http = inject(HttpClient);
  private readonly base = 'http://localhost:7000';

  /**
   * Upload an image file to the backend.
   * Returns the public URL string (e.g. /uploads/abc123.jpg).
   */
  uploadImage(file: File): Observable<string> {
    const form = new FormData();
    form.append('file', file);

    return this.http
      .post<{ url: string }>(`${this.base}/uploads/image`, form)
      .pipe(map(res => `${this.base}${res.url}`));
  }
}
