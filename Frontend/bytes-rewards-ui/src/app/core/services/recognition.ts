import { Injectable } from '@angular/core';

export interface Recognition {
  from: string;
  to: string;
  message: string;
  bytes: number;
}

@Injectable({
  providedIn: 'root'
})
export class RecognitionService {

  private recognitions: Recognition[] = [];

  giveRecognition(
    recognition: Recognition
  ): void {

    this.recognitions.push(recognition);
  }

  getRecognitions(): Recognition[] {

    return this.recognitions;
  }
}