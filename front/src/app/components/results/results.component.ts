import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ResultRecord } from '../../models/models';

@Component({
    selector: 'app-results',
    templateUrl: './results.component.html',
    styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit {
    results: ResultRecord[] = [];
    loading = false;
    errorMessage: string | null = null;
    selectedFile: string = '';
    
    filters = {
        fileName: '',
        minDate: '',
        maxDate: '',
        minAvgValue: null as number | null,
        maxAvgValue: null as number | null,
        minAvgExecTime: null as number | null,
        maxAvgExecTime: null as number | null
    };

    constructor(private apiService: ApiService) {}

    ngOnInit(): void {
        this.loadResults();
    }

    loadResults(): void {
        this.loading = true;
        this.errorMessage = null;
        
        const filters: any = { ...this.filters };
        Object.keys(filters).forEach(key => {
            const value = filters[key];
            if (value === '' || value === null || value === undefined) {
                delete filters[key];
            }
        });

        this.apiService.getResults(filters).subscribe({
            next: (data: ResultRecord[]) => {
                this.results = data;
                this.loading = false;
            },
            error: (error: any) => {
                this.errorMessage = 'Ошибка при загрузке результатов';
                this.loading = false;
                console.error('Results error:', error);
            }
        });
    }

    applyFilters(): void {
        this.loadResults();
    }

    resetFilters(): void {
        this.filters = {
            fileName: '',
            minDate: '',
            maxDate: '',
            minAvgValue: null,
            maxAvgValue: null,
            minAvgExecTime: null,
            maxAvgExecTime: null
        };
        this.loadResults();
    }

    getLastValues(fileName: string): void {
        this.selectedFile = fileName;
        console.log('View last 10 values for:', fileName);
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