import 'package:easy_localization/easy_localization.dart';
import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/repositories/report_repository.dart';
import '../bloc/reports_bloc.dart';
import '../bloc/reports_event.dart';
import '../bloc/reports_state.dart';

class ReportsScreen extends StatefulWidget {
  const ReportsScreen({super.key});

  @override
  State<ReportsScreen> createState() => _ReportsScreenState();
}

class _ReportsScreenState extends State<ReportsScreen> {
  DateTime _from = DateTime.now().subtract(const Duration(days: 30));
  DateTime _to = DateTime.now();

  @override
  void initState() {
    super.initState();
    _load();
  }

  void _load() {
    context.read<ReportsBloc>().add(
          ReportsSummaryRequested(from: _from, to: _to),
        );
  }

  Future<void> _pickDateRange() async {
    final range = await showDateRangePicker(
      context: context,
      firstDate: DateTime(2024),
      lastDate: DateTime.now(),
      initialDateRange: DateTimeRange(start: _from, end: _to),
    );
    if (range == null) return;
    setState(() {
      _from = range.start;
      _to = range.end;
    });
    _load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('reports.title'.tr()),
        actions: [
          IconButton(
            icon: const Icon(Icons.date_range),
            onPressed: _pickDateRange,
          ),
        ],
      ),
      body: BlocConsumer<ReportsBloc, ReportsState>(
        listener: (ctx, state) {
          if (state is ReportDownloaded) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text('reports.downloaded'.tr())),
            );
          }
        },
        builder: (ctx, state) {
          if (state is ReportsLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is ReportsSummaryLoaded) {
            return _buildContent(ctx, state.entries);
          }
          return Center(child: Text('common.no_data'.tr()));
        },
      ),
    );
  }

  Widget _buildContent(BuildContext ctx, List<AttendanceSummaryEntry> entries) {
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        if (entries.isNotEmpty)
          SizedBox(
            height: 200,
            child: BarChart(
              BarChartData(
                barGroups: entries.asMap().entries.map((e) {
                  return BarChartGroupData(
                    x: e.key,
                    barRods: [
                      BarChartRodData(
                        toY: e.value.present.toDouble(),
                        color: Theme.of(ctx).colorScheme.primary,
                        width: 12,
                      ),
                    ],
                  );
                }).toList(),
                gridData: const FlGridData(show: false),
                borderData: FlBorderData(show: false),
                titlesData: const FlTitlesData(show: false),
              ),
            ),
          ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                icon: const Icon(Icons.picture_as_pdf),
                label: Text('reports.download_pdf'.tr()),
                onPressed: () => ctx.read<ReportsBloc>().add(
                      ReportDownloadRequested(format: 'pdf', from: _from, to: _to),
                    ),
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: OutlinedButton.icon(
                icon: const Icon(Icons.table_chart),
                label: Text('reports.download_excel'.tr()),
                onPressed: () => ctx.read<ReportsBloc>().add(
                      ReportDownloadRequested(format: 'xlsx', from: _from, to: _to),
                    ),
              ),
            ),
          ],
        ),
      ],
    );
  }
}
