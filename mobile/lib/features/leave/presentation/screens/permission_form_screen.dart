import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../domain/entities/leave_request.dart';
import '../bloc/leave_bloc.dart';
import '../bloc/leave_event.dart';
import '../bloc/leave_state.dart';

class PermissionFormScreen extends StatefulWidget {
  const PermissionFormScreen({super.key});

  @override
  State<PermissionFormScreen> createState() => _PermissionFormScreenState();
}

class _PermissionFormScreenState extends State<PermissionFormScreen> {
  final _formKey = GlobalKey<FormState>();
  DateTime? _fromDate;
  DateTime? _toDate;
  final _reasonCtrl = TextEditingController();

  @override
  void dispose() {
    _reasonCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDateTime(bool isFrom) async {
    final date = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 90)),
    );
    if (date == null || !mounted) return;

    final time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    if (time == null) return;

    final combined =
        DateTime(date.year, date.month, date.day, time.hour, time.minute);
    setState(() {
      if (isFrom) _fromDate = combined;
      else _toDate = combined;
    });
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    if (_fromDate == null || _toDate == null) return;

    context.read<LeaveBloc>().add(LeaveSubmitted(
          type: LeaveType.permission,
          fromDate: _fromDate!,
          toDate: _toDate!,
          reason: _reasonCtrl.text.isEmpty ? null : _reasonCtrl.text,
        ));
  }

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('yyyy-MM-dd HH:mm');

    return Scaffold(
      appBar: AppBar(title: Text('leave.permission'.tr())),
      body: BlocListener<LeaveBloc, LeaveState>(
        listener: (ctx, state) {
          if (state is LeaveSubmitSuccess) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text('leave.submitted_success'.tr())),
            );
            ctx.pop();
          } else if (state is LeaveFailure) {
            ScaffoldMessenger.of(ctx).showSnackBar(
              SnackBar(content: Text(state.failure.message)),
            );
          }
        },
        child: Form(
          key: _formKey,
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                InkWell(
                  onTap: () => _pickDateTime(true),
                  child: InputDecorator(
                    decoration: InputDecoration(
                      labelText: 'leave.from_date'.tr(),
                      border: const OutlineInputBorder(),
                    ),
                    child: Text(_fromDate != null ? fmt.format(_fromDate!) : '—'),
                  ),
                ),
                const SizedBox(height: 12),
                InkWell(
                  onTap: () => _pickDateTime(false),
                  child: InputDecorator(
                    decoration: InputDecoration(
                      labelText: 'leave.to_date'.tr(),
                      border: const OutlineInputBorder(),
                    ),
                    child: Text(_toDate != null ? fmt.format(_toDate!) : '—'),
                  ),
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _reasonCtrl,
                  maxLines: 3,
                  decoration: InputDecoration(
                    labelText: 'leave.reason'.tr(),
                    border: const OutlineInputBorder(),
                  ),
                ),
                const SizedBox(height: 24),
                BlocBuilder<LeaveBloc, LeaveState>(
                  builder: (_, state) => ElevatedButton(
                    onPressed: state is LeaveLoading ? null : _submit,
                    child: state is LeaveLoading
                        ? const CircularProgressIndicator()
                        : Text('common.submit'.tr()),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
