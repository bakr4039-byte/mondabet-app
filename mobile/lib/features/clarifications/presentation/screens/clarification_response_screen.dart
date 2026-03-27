import 'package:dio/dio.dart';
import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/di/injection.dart';
import '../../../../core/network/api_client.dart';
import '../../../clarifications/domain/entities/clarification.dart';

class ClarificationResponseScreen extends StatefulWidget {
  final ClarificationItem clarification;
  const ClarificationResponseScreen({super.key, required this.clarification});

  @override
  State<ClarificationResponseScreen> createState() =>
      _ClarificationResponseScreenState();
}

class _ClarificationResponseScreenState
    extends State<ClarificationResponseScreen> {
  final _ctrl = TextEditingController();
  bool _loading = false;

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_ctrl.text.trim().isEmpty) return;
    setState(() => _loading = true);

    try {
      final dio = getIt<ApiClient>().dio;
      await dio.put<void>(
        '/clarifications/${widget.clarification.id}/respond',
        data: {'responseText': _ctrl.text.trim()},
      );
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('clarification.submitted'.tr())),
        );
        context.pop();
      }
    } on DioException catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.message ?? 'common.error.generic'.tr())),
        );
      }
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = widget.clarification;
    final fmt = DateFormat('yyyy-MM-dd');

    return Scaffold(
      appBar: AppBar(title: Text('clarification.respond'.tr())),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Card(
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      '${fmt.format(c.fromDate)} → ${fmt.format(c.toDate)}',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                    const SizedBox(height: 8),
                    Text(c.question, style: Theme.of(context).textTheme.bodyLarge),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
            if (c.status == ClarificationStatus.responded)
              Card(
                color: Colors.green.shade50,
                child: Padding(
                  padding: const EdgeInsets.all(12),
                  child: Text(c.responseText ?? ''),
                ),
              )
            else ...[
              TextField(
                controller: _ctrl,
                maxLines: 4,
                decoration: InputDecoration(
                  labelText: 'clarification.your_response'.tr(),
                  border: const OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _loading ? null : _submit,
                child: _loading
                    ? const CircularProgressIndicator()
                    : Text('common.submit'.tr()),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
