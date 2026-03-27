import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../domain/entities/leave_request.dart';
import '../bloc/leave_bloc.dart';
import '../bloc/leave_event.dart';
import '../bloc/leave_state.dart';

class VacationFormScreen extends StatefulWidget {
  const VacationFormScreen({super.key});

  @override
  State<VacationFormScreen> createState() => _VacationFormScreenState();
}

class _VacationFormScreenState extends State<VacationFormScreen> {
  final _formKey = GlobalKey<FormState>();
  DateTime? _fromDate;
  DateTime? _toDate;
  final _reasonCtrl = TextEditingController();

  @override
  void dispose() {
    _reasonCtrl.dispose();
    super.dispose();
  }

  Future<void> _pickDate(bool isFrom) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked == null) return;
    setState(() {
      if (isFrom) {
        _fromDate = picked;
      } else {
        _toDate = picked;
      }
    });
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    if (_fromDate == null || _toDate == null) return;

    context.read<LeaveBloc>().add(LeaveSubmitted(
          type: LeaveType.vacation,
          fromDate: _fromDate!,
          toDate: _toDate!,
          reason: _reasonCtrl.text.isEmpty ? null : _reasonCtrl.text,
        ));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('leave.vacation'.tr())),
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
                _DateField(
                  label: 'leave.from_date'.tr(),
                  value: _fromDate,
                  onTap: () => _pickDate(true),
                ),
                const SizedBox(height: 12),
                _DateField(
                  label: 'leave.to_date'.tr(),
                  value: _toDate,
                  onTap: () => _pickDate(false),
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

class _DateField extends StatelessWidget {
  final String label;
  final DateTime? value;
  final VoidCallback onTap;
  const _DateField({required this.label, required this.value, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final fmt = DateFormat('yyyy-MM-dd');
    return InkWell(
      onTap: onTap,
      child: InputDecorator(
        decoration: InputDecoration(
          labelText: label,
          border: const OutlineInputBorder(),
          suffixIcon: const Icon(Icons.calendar_today),
        ),
        child: Text(value != null ? fmt.format(value!) : '—'),
      ),
    );
  }
}
