import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../domain/entities/leave_request.dart';
import '../bloc/leave_bloc.dart';
import '../bloc/leave_event.dart';
import '../bloc/leave_state.dart';

class LeaveListScreen extends StatefulWidget {
  const LeaveListScreen({super.key});

  @override
  State<LeaveListScreen> createState() => _LeaveListScreenState();
}

class _LeaveListScreenState extends State<LeaveListScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabs;

  @override
  void initState() {
    super.initState();
    _tabs = TabController(length: 3, vsync: this);
    context.read<LeaveBloc>().add(const LeavesLoaded());
  }

  @override
  void dispose() {
    _tabs.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('leave.my_leaves'.tr()),
        bottom: TabBar(
          controller: _tabs,
          tabs: [
            Tab(text: 'leave.status.pending'.tr()),
            Tab(text: 'leave.status.approved'.tr()),
            Tab(text: 'leave.status.rejected'.tr()),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        icon: const Icon(Icons.add),
        label: Text('leave.new_request'.tr()),
        onPressed: () => _showLeaveTypeSheet(context),
      ),
      body: BlocBuilder<LeaveBloc, LeaveState>(
        builder: (ctx, state) {
          if (state is LeaveLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is LeaveListLoaded) {
            return TabBarView(
              controller: _tabs,
              children: [
                _buildList(state.leaves, LeaveStatus.pending),
                _buildList(state.leaves, LeaveStatus.approved),
                _buildList(state.leaves, LeaveStatus.rejected),
              ],
            );
          }
          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildList(List<LeaveRequest> all, LeaveStatus status) {
    final filtered = all.where((l) => l.status == status).toList();
    if (filtered.isEmpty) {
      return Center(child: Text('common.no_data'.tr()));
    }
    return RefreshIndicator(
      onRefresh: () async =>
          context.read<LeaveBloc>().add(const LeavesLoaded()),
      child: ListView.separated(
        padding: const EdgeInsets.all(12),
        itemCount: filtered.length,
        separatorBuilder: (_, __) => const SizedBox(height: 8),
        itemBuilder: (_, i) => _LeaveCard(leave: filtered[i]),
      ),
    );
  }

  void _showLeaveTypeSheet(BuildContext ctx) {
    showModalBottomSheet<void>(
      context: ctx,
      builder: (_) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.beach_access),
              title: Text('leave.vacation'.tr()),
              onTap: () {
                Navigator.pop(ctx);
                ctx.push('/leaves/vacation');
              },
            ),
            ListTile(
              leading: const Icon(Icons.access_time),
              title: Text('leave.permission'.tr()),
              onTap: () {
                Navigator.pop(ctx);
                ctx.push('/leaves/permission');
              },
            ),
            ListTile(
              leading: const Icon(Icons.note_add),
              title: Text('leave.excuse'.tr()),
              onTap: () {
                Navigator.pop(ctx);
                ctx.push('/leaves/excuse');
              },
            ),
          ],
        ),
      ),
    );
  }
}

class _LeaveCard extends StatelessWidget {
  final LeaveRequest leave;
  const _LeaveCard({required this.leave});

  @override
  Widget build(BuildContext context) {
    final statusColor = switch (leave.status) {
      LeaveStatus.approved => Colors.green,
      LeaveStatus.rejected => Colors.red,
      LeaveStatus.pending => Colors.orange,
    };
    final typeLabel = switch (leave.type) {
      LeaveType.vacation => 'leave.vacation'.tr(),
      LeaveType.permission => 'leave.permission'.tr(),
      LeaveType.excuse => 'leave.excuse'.tr(),
    };
    final fmt = DateFormat('yyyy-MM-dd');

    return Card(
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: statusColor.withOpacity(0.15),
          child: Icon(Icons.event_note, color: statusColor),
        ),
        title: Text(typeLabel),
        subtitle: Text(
          '${fmt.format(leave.fromDate)} → ${fmt.format(leave.toDate)}',
        ),
        trailing: Chip(
          label: Text(
            'leave.status.${leave.status.name}'.tr(),
            style: const TextStyle(fontSize: 11),
          ),
          backgroundColor: statusColor.withOpacity(0.1),
        ),
      ),
    );
  }
}
