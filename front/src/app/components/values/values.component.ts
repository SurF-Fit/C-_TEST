import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ValueRecord } from '../../models/models';

@Component({
    selector: 'app-values',
    templateUrl: './values.component.html',
    styleUrls: ['./values.component.css']
})
export class ValuesComponent {
    fileName: string = '';
    values: ValueRecord[] = [];
    loading = false;
    errorMessage: string | null = null;
    showModal = false;

    constructor(private apiService: ApiService) {}

    loadValues(): void {
        if (!this.fileName.trim()) {
            this.errorMessage = 'Введите имя файла';
            return;
        }

        this.loading = true;
        this.errorMessage = null;
        this.values = [];

        this.apiService.getLast10Values(this.fileName).subscribe({
            next: (data: ValueRecord[]) => {
                this.values = data;
                this.loading = false;
                this.showModal = true;
            },
            error: (error: any) => {
                if (error.status === 404) {
                    this.errorMessage = `Файл "${this.fileName}" не найден`;
                } else {
                    this.errorMessage = 'Ошибка при загрузке значений';
                }
                this.loading = false;
                console.error('Values error:', error);
            }
        });
    }

    openModal(fileName: string): void {
        this.fileName = fileName;
        this.loadValues();
    }

    closeModal(): void {
        this.showModal = false;
        this.values = [];
        this.fileName = '';
    }

    // Метод для форматирования чисел
    formatNumber(value: number): string {
        if (value === undefined || value === null) return '0';
        return value.toFixed(2);
    }

    // Метод для форматирования дат
    formatDate(date: string | Date): string {
        if (!date) return '';
        return new Date(date).toLocaleString('ru-RU', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    }
}